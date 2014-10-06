using System;
using System.IO;
using System.Linq;
using System.Threading;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Events;
using Ionic.Zip;
using DiagnosticsModule.Models;
using FiresecAPI.Journal;

namespace DiagnosticsModule.ViewModels
{
	public class DiagnosticsViewModel : ViewPartViewModel
	{
		int Count = 0;

		public DiagnosticsViewModel()
		{
			AddJournalCommand = new RelayCommand(OnAddJournal);
			SaveCommand = new RelayCommand(OnSave);
			LoadCommand = new RelayCommand(OnLoad);
		}

		public RelayCommand AddJournalCommand { get; private set; }
		void OnAddJournal()
		{
			var journalItem = new JournalItem();
			journalItem.DeviceDateTime = DateTime.Now;
			journalItem.JournalEventNameType = JournalEventNameType.Подтверждение_тревоги;
			FiresecManager.FiresecService.AddJournalItem(journalItem);
		}

		public RelayCommand SaveCommand { get; private set; }
		void OnSave()
		{
			SerializerHelper.Save();
		}

		public RelayCommand LoadCommand { get; private set; }
		void OnLoad()
		{
			SerializerHelper.Load();
		}


		public void StopThreads()
		{
			IsThreadStoping = true;
		}
		bool IsThreadStoping = false;

		string _text;
		public string Text
		{
			get { return _text; }
			set
			{
				_text = value;
				OnPropertyChanged(() => Text);
			}
		}

		public static bool SetNewConfig()
		{
			ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Publish(null);

			WaitHelper.Execute(() =>
			{
				LoadingService.Show("Применение конфигурации", "Применение конфигурации", 10);
				if (ServiceFactory.SaveService.FSChanged || ServiceFactory.SaveService.FSParametersChanged)
				{
					if (!GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1)
					{
						LoadingService.DoStep("Применение конфигурации устройств");
						if (FiresecManager.FiresecDriver != null)
						{
							var fsResult = FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
							LoadingService.DoStep("Синхронизация конфигурации");
							FiresecManager.FiresecDriver.Synchronyze(false);
						}
					}
				}

				var tempFileName = SaveAllConfigToFile();
				using (var fileStream = new FileStream(tempFileName, FileMode.Open))
				{
					FiresecManager.FiresecService.SetConfig(fileStream);
				}
				File.Delete(tempFileName);

				FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
			});
			LoadingService.Close();
			ServiceFactory.SaveService.Reset();
			return true;
		}

		public static string SaveAllConfigToFile(bool saveAnyway = false)
		{
			var tempFolderName = AppDataFolderHelper.GetTempFolder();
			if (!Directory.Exists(tempFolderName))
				Directory.CreateDirectory(tempFolderName);

			var tempFileName = AppDataFolderHelper.GetTempFileName();
			if (File.Exists(tempFileName))
				File.Delete(tempFileName);

			TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

			if (ServiceFactory.SaveService.FSChanged || ServiceFactory.SaveService.FSParametersChanged || saveAnyway)
				AddConfiguration(tempFolderName, "DeviceConfiguration.xml", FiresecManager.FiresecConfiguration.DeviceConfiguration, 1, 1);
			if (ServiceFactory.SaveService.PlansChanged || saveAnyway)
				AddConfiguration(tempFolderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1);
			if (ServiceFactory.SaveService.InstructionsChanged || ServiceFactory.SaveService.SoundsChanged || ServiceFactory.SaveService.FilterChanged || ServiceFactory.SaveService.CamerasChanged || ServiceFactory.SaveService.EmailsChanged || ServiceFactory.SaveService.AutomationChanged || saveAnyway)
				AddConfiguration(tempFolderName, "SystemConfiguration.xml", FiresecManager.SystemConfiguration, 1, 1);
			if (ServiceFactory.SaveService.GKChanged || ServiceFactory.SaveService.XInstructionsChanged || saveAnyway)
				AddConfiguration(tempFolderName, "GKDeviceConfiguration.xml", GKManager.DeviceConfiguration, 1, 1);
			AddConfiguration(tempFolderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1);
			if (ServiceFactory.SaveService.SecurityChanged || saveAnyway)
				AddConfiguration(tempFolderName, "SecurityConfiguration.xml", FiresecManager.SecurityConfiguration, 1, 1);
			if (ServiceFactory.SaveService.LibraryChanged || saveAnyway)
				AddConfiguration(tempFolderName, "DeviceLibraryConfiguration.xml", FiresecManager.DeviceLibraryConfiguration, 1, 1);
			if (ServiceFactory.SaveService.GKLibraryChanged || saveAnyway)
				AddConfiguration(tempFolderName, "GKDeviceLibraryConfiguration.xml", GKManager.DeviceLibraryConfiguration, 1, 1);
			if (ServiceFactory.SaveService.SKDChanged || saveAnyway)
				AddConfiguration(tempFolderName, "SKDConfiguration.xml", SKDManager.SKDConfiguration, 1, 1);
			if (ServiceFactory.SaveService.SKDLibraryChanged || saveAnyway)
				AddConfiguration(tempFolderName, "SKDLibraryConfiguration.xml", SKDManager.SKDLibraryConfiguration, 1, 1);
			if (ServiceFactory.SaveService.LayoutsChanged || saveAnyway)
				AddConfiguration(tempFolderName, "LayoutsConfiguration.xml", FiresecManager.LayoutsConfiguration, 1, 1);

			var destinationImagesDirectory = AppDataFolderHelper.GetFolder(Path.Combine(tempFolderName, "Content"));
			if (Directory.Exists(ServiceFactory.ContentService.ContentFolder))
			{
				if (Directory.Exists(destinationImagesDirectory))
					Directory.Delete(destinationImagesDirectory);
				if (!Directory.Exists(destinationImagesDirectory))
					Directory.CreateDirectory(destinationImagesDirectory);
				var sourceImagesDirectoryInfo = new DirectoryInfo(ServiceFactory.ContentService.ContentFolder);
				foreach (var fileInfo in sourceImagesDirectoryInfo.GetFiles())
				{
					fileInfo.CopyTo(Path.Combine(destinationImagesDirectory, fileInfo.Name));
				}
			}

			var zipFile = new ZipFile(tempFileName);
			zipFile.AddDirectory(tempFolderName);
			zipFile.Save(tempFileName);
			zipFile.Dispose();
			if (Directory.Exists(tempFolderName))
				Directory.Delete(tempFolderName, true);

			return tempFileName;
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name), true);

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}
	}
}