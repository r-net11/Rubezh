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
					var tempFolderName = AppDataFolderHelper.GetTempFolder();
					if (!Directory.Exists(tempFolderName))
						Directory.CreateDirectory(tempFolderName);

					var tempFileName = AppDataFolderHelper.GetTempFileName();
					if (File.Exists(tempFileName))
						File.Delete(tempFileName);

					TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

					AddConfiguration(tempFolderName, "DeviceConfiguration.xml", FiresecManager.FiresecConfiguration.DeviceConfiguration, 1, 1);
					AddConfiguration(tempFolderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1);
					AddConfiguration(tempFolderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1);

					var zipFile = new ZipFile(tempFileName);
					zipFile.AddDirectory(tempFolderName);
					zipFile.Save(tempFileName);
					zipFile.Dispose();
					if (Directory.Exists(tempFolderName))
						Directory.Delete(tempFolderName, true);

					if (Directory.Exists(tempFolderName))
						Directory.Delete(tempFolderName, true);

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

		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name));

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
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