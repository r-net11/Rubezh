using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using FiresecAPI.Journal;

namespace FireAdministrator
{
	public static class ConfigManager
	{
		public static bool SetNewConfig()
		{
			try
			{
				ServiceFactory.Events.GetEvent<BeforeConfigurationSerializeEvent>().Publish(null);
				ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Publish(null);

				var validationResult = ServiceFactory.ValidationService.Validate();
				if (validationResult.HasErrors())
				{
					if (validationResult.CannotSave())
					{
						MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
						return false;
					}

					if (!MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?"))
						return false;
				}

				WaitHelper.Execute(() =>
				{
					FiresecManager.FiresecService.GKAddMessage(JournalEventNameType.Применение_конфигурации, "");
					LoadingService.Show("Применение конфигурации", "Применение конфигурации", 10);

					if (ConnectionSettingsManager.IsRemote)
					{
						var tempFileName = SaveConfigToFile(false);
						using (var fileStream = new FileStream(tempFileName, FileMode.Open))
						{
							FiresecManager.FiresecService.SetRemoteConfig(fileStream);
						}
						File.Delete(tempFileName);
					}
					else
					{
					    SaveConfigToFile(true);
					    FiresecManager.FiresecService.SetLocalConfig();
					}
				});
				LoadingService.Close();
				ServiceFactory.SaveService.Reset();
				return true;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SetNewConfig");
				MessageBoxService.ShowError(e.Message, "Ошибка при выполнении операции");
				return false;
			}
		}

		static string SaveConfigToFile(bool isLocal)
		{
			try
			{
				var tempFolderName = "";
				if (isLocal)
				{
					tempFolderName = AppDataFolderHelper.GetServerAppDataPath("Config");
				}
				else
				{
					tempFolderName = AppDataFolderHelper.GetTempFolder();
				}

				if (!Directory.Exists(tempFolderName))
					Directory.CreateDirectory(tempFolderName);

				TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

				if (ServiceFactory.SaveService.PlansChanged)
					AddConfiguration(tempFolderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.SoundsChanged || ServiceFactory.SaveService.FilterChanged || ServiceFactory.SaveService.CamerasChanged || ServiceFactory.SaveService.EmailsChanged || ServiceFactory.SaveService.AutomationChanged)
					AddConfiguration(tempFolderName, "SystemConfiguration.xml", FiresecManager.SystemConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.GKChanged)
					AddConfiguration(tempFolderName, "GKDeviceConfiguration.xml", GKManager.DeviceConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.SecurityChanged)
					AddConfiguration(tempFolderName, "SecurityConfiguration.xml", FiresecManager.SecurityConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.GKLibraryChanged)
					AddConfiguration(tempFolderName, "GKDeviceLibraryConfiguration.xml", GKManager.DeviceLibraryConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.LayoutsChanged)
					AddConfiguration(tempFolderName, "LayoutsConfiguration.xml", FiresecManager.LayoutsConfiguration, 1, 1, false);

				if (!isLocal)
				{
					AddConfiguration(tempFolderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1, true);
				}

				var destinationImagesDirectory = AppDataFolderHelper.GetFolder(Path.Combine(tempFolderName, "Content"));
				if (Directory.Exists(ServiceFactory.ContentService.ContentFolder))
				{
					if (Directory.Exists(destinationImagesDirectory))
						Directory.Delete(destinationImagesDirectory, true);
					if (!Directory.Exists(destinationImagesDirectory))
						Directory.CreateDirectory(destinationImagesDirectory);
					var sourceImagesDirectoryInfo = new DirectoryInfo(ServiceFactory.ContentService.ContentFolder);
					foreach (var fileInfo in sourceImagesDirectoryInfo.GetFiles())
					{
						fileInfo.CopyTo(Path.Combine(destinationImagesDirectory, fileInfo.Name));
					}
				}

				if (!isLocal)
				{
					var tempFileName = AppDataFolderHelper.GetTempFileName();
					if (File.Exists(tempFileName))
						File.Delete(tempFileName);

					var zipFile = new ZipFile(tempFileName);
					zipFile.AddDirectory(tempFolderName);
					zipFile.Save(tempFileName);
					zipFile.Dispose();
					if (Directory.Exists(tempFolderName))
						Directory.Delete(tempFolderName, true);

					return tempFileName;
				}
				return null;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigManager.SaveAllConfigToFile");
			}
			return null;
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion, bool useXml)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			var filePath = Path.Combine(folderName, name);
			if (File.Exists(filePath))
				File.Delete(filePath);
			ZipSerializeHelper.Serialize(configuration, filePath, useXml);
			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		public static void CreateNew()
		{
			try
			{
				if (MessageBoxService.ShowQuestion("Вы уверены, что хотите создать новую конфигурацию"))
				{
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
					ServiceFactory.ContentService.Clear();
					GKManager.SetEmptyConfiguration();
					FiresecManager.PlansConfiguration = new PlansConfiguration();
					FiresecManager.SystemConfiguration.Cameras = new List<Camera>();
					FiresecManager.SystemConfiguration.AutomationConfiguration = new AutomationConfiguration();
					FiresecManager.PlansConfiguration.Update();
					FiresecManager.LayoutsConfiguration = new LayoutsConfiguration();

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

					ServiceFactory.SaveService.PlansChanged = true;
					ServiceFactory.SaveService.CamerasChanged = true;
					ServiceFactory.SaveService.GKChanged = true;
					ServiceFactory.SaveService.LayoutsChanged = true;

					ShowFirstDevice();
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.CreateNew");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}

		public static void ShowFirstDevice()
		{
			ServiceFactory.Layout.Close();
			ServiceFactory.Layout.ShowFooter(null);
			if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
			{
				var deviceUID = Guid.Empty;
				var firstDevice = GKManager.Devices.FirstOrDefault();
				if (firstDevice != null)
					deviceUID = firstDevice.UID;
				ServiceFactory.Events.GetEvent<ShowGKDeviceEvent>().Publish(deviceUID);
			}
		}
	}
}