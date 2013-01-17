using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure;
using System.IO;
using FiresecClient;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using FiresecAPI;
using FiresecAPI.Models;
using System.Windows;

namespace SettingsModule.ViewModels
{
	public class ConvertationViewModel : BaseViewModel
	{
		public ConvertationViewModel()
		{
			ConvertConfigurationCommand = new RelayCommand(OnConvertConfiguration, CanConvertConfiguration);
			ConvertJournalCommand = new RelayCommand(OnConvertJournal, CanConvertJournal);
		}

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
		bool CanConvertConfiguration()
		{
			return FiresecManager.FiresecDriver != null;
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
		bool CanConvertJournal()
		{
			return FiresecManager.FiresecDriver != null;
		}
	}
}