using System;
using System.Linq;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Microsoft.Win32;
using Ionic.Zip;

namespace SettingsModule.ViewModels
{
	public class SettingsViewModel : ViewPartViewModel
	{
		public SettingsViewModel()
		{
			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal);
            LoadFromFileOldCommand = new RelayCommand(OnLoadFromFileOld);
            SaveToFileOldCommand = new RelayCommand(OnSaveToFileOld);
		}
        public void Initialize()
        {
            ThemeContext = new ThemeViewModel();
            ModuleContext = new ModuleViewModel();
        }

        public ThemeViewModel ThemeContext { get; set; }
        public ModuleViewModel ModuleContext { get; set; }
        public RelayCommand ConvertConfigurationCommand { get; private set; }
		void OnConvertConfiguration()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать конфигурацию?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					LoadingService.ShowProgress("Конвертирование конфигурации", "Конвертирование конфигурации", 6);
					var convertstionResult = FiresecManager.FiresecDriver.Convert();
					if (convertstionResult.HasError)
					{
						MessageBoxService.ShowError(convertstionResult.Error);
						return;
					}
					ServiceFactory.SaveService.FSChanged = false;
					ServiceFactory.SaveService.PlansChanged = false;
					LoadingService.DoStep("Обновление конфигурации");
					FiresecManager.UpdateConfiguration();

					LoadingService.DoStep("Сохранение конфигурации");
					var tempFileName = Path.GetTempFileName() + "_";
					var zipFile = new ZipFile(tempFileName);
					TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
					AddConfiguration(zipFile, FiresecManager.FiresecConfiguration.DeviceConfiguration, "DeviceConfiguration.xml", 1, 1);
					AddConfiguration(zipFile, FiresecManager.PlansConfiguration, "PlansConfiguration.xml", 1, 1);
					AddConfiguration(zipFile, TempZipConfigurationItemsCollection, "ZipConfigurationItemsCollection.xml", 1, 1);
					zipFile.Save(tempFileName);
					zipFile.Dispose();
					using (var fileStream = new FileStream(tempFileName, FileMode.Open))
					{
						FiresecManager.FiresecService.SetConfig(fileStream);
					}
					File.Delete(tempFileName);

					LoadingService.DoStep("Оповещение клиентов об изменении конфигурации");
					FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
					LoadingService.Close();
				});
				ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
			}
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(ZipFile zipFile, VersionedConfiguration configuration, string name, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			var configurationStream = ZipSerializeHelper.Serialize(configuration);
			if (zipFile.Entries.Any(x => x.FileName == name))
				zipFile.RemoveEntry(name);
			configurationStream.Position = 0;
			zipFile.AddEntry(name, configurationStream);
		}

        public RelayCommand LoadFromFileOldCommand { get; private set; }
        public static void OnLoadFromFileOld()
        {
            try
            {
                var openDialog = new OpenFileDialog()
                {
                    Filter = "firesec2 files|*.fsc2",
                    DefaultExt = "firesec2 files|*.fsc2"
                };
                if (openDialog.ShowDialog().Value)
                {
                    WaitHelper.Execute(() =>
                    {
                        CopyTo(LoadFromFile(openDialog.FileName));

                        FiresecManager.UpdateConfiguration();
                        XManager.UpdateConfiguration();
                        ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

                        ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
                        ServiceFactory.Layout.Close();
                        ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);

                        ServiceFactory.SaveService.FSChanged = true;
                        ServiceFactory.SaveService.PlansChanged = true;
                        ServiceFactory.SaveService.GKChanged = true;
                        ServiceFactory.Layout.ShowFooter(null);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.LoadFromFileOld");
                MessageBox.Show(e.Message, "Ошибка при выполнении операции");
            }
        }
        public RelayCommand SaveToFileOldCommand { get; private set; }
        public static void OnSaveToFileOld()
        {
            try
            {
                var saveDialog = new SaveFileDialog()
                {
                    Filter = "firesec2 files|*.fsc2",
                    DefaultExt = "firesec2 files|*.fsc2"
                };
                if (saveDialog.ShowDialog().Value)
                {
                    WaitHelper.Execute(() =>
                    {
                        SaveToFile(CopyFrom(), saveDialog.FileName);
                    });
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.SaveToFile");
                MessageBox.Show(e.Message, "Ошибка при выполнении операции");
            }
        }
        static FullConfiguration LoadFromFile(string fileName)
        {
            try
            {
                FullConfiguration fullConfiguration = null;
                var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
                using (var fileStream = new FileStream(fileName, FileMode.Open))
                {
                    fullConfiguration = (FullConfiguration)dataContractSerializer.ReadObject(fileStream);
                }
                if (!fullConfiguration.ValidateVersion())
                    SaveToFile(fullConfiguration, fileName);
                return fullConfiguration;
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.LoadFromFile");
                throw e;
            }
        }
        static void SaveToFile(FullConfiguration fullConfiguration, string fileName)
        {
            try
            {
                var dataContractSerializer = new DataContractSerializer(typeof(FullConfiguration));
                using (var fileStream = new FileStream(fileName, FileMode.Create))
                {
                    dataContractSerializer.WriteObject(fileStream, fullConfiguration);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.SaveToFile");
                throw e;
            }
        }
        static void CopyTo(FullConfiguration fullConfiguration)
        {
            try
            {
                FiresecManager.FiresecConfiguration.DeviceConfiguration = fullConfiguration.DeviceConfiguration;
                if (FiresecManager.FiresecConfiguration.DeviceConfiguration == null)
                    FiresecManager.FiresecConfiguration.SetEmptyConfiguration();
                FiresecManager.PlansConfiguration = fullConfiguration.PlansConfiguration;
                FiresecManager.SystemConfiguration = fullConfiguration.SystemConfiguration;
                XManager.DeviceConfiguration = fullConfiguration.XDeviceConfiguration;
                if (XManager.DeviceConfiguration == null)
                    XManager.SetEmptyConfiguration();
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.CopyTo");
                throw e;
            }
        }
        static FullConfiguration CopyFrom()
        {
            try
            {
                return new FullConfiguration()
                {
                    DeviceConfiguration = FiresecManager.FiresecConfiguration.DeviceConfiguration,
                    PlansConfiguration = FiresecManager.PlansConfiguration,
                    SystemConfiguration = FiresecManager.SystemConfiguration,
                    XDeviceConfiguration = XManager.DeviceConfiguration,
                    Version = new ConfigurationVersion() { MajorVersion = 1, MinorVersion = 1 }
                };
            }
            catch (Exception e)
            {
                Logger.Error(e, "MenuView.CopyFrom");
                throw e;
            }
        }

		public RelayCommand ConvertJournalCommand { get; private set; }
		void OnConvertJournal()
		{
			if (MessageBoxService.ShowQuestion("Вы уверены, что хотите конвертировать журнал событий?") == MessageBoxResult.Yes)
			{
				WaitHelper.Execute(() =>
				{
					var journalRecords = FiresecManager.FiresecDriver.ConvertJournal();
					FiresecManager.FiresecService.SetJournal(journalRecords);
				});
			}
		}

	}
}