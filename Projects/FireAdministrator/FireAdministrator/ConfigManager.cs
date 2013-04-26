using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;

namespace FireAdministrator
{
	public static class ConfigManager
	{
		public static bool SetNewConfig()
		{
			try
			{
				ServiceFactory.Events.GetEvent<ConfigurationSavingEvent>().Publish(null);

				var validationResult = ServiceFactory.ValidationService.Validate();
				if (validationResult.HasErrors())
				{
					if (validationResult.CannotSave())
					{
						MessageBoxService.ShowWarning("Обнаружены ошибки. Операция прервана");
						return false;
					}

					if (MessageBoxService.ShowQuestion("Конфигурация содержит ошибки. Продолжить?") != MessageBoxResult.Yes)
						return false;
				}

				WaitHelper.Execute(() =>
				{
					LoadingService.ShowProgress("Применение конфигурации", "Применение конфигурации", 10);
					if (ServiceFactory.SaveService.FSChanged)
					{
						if (!GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1)
						{
							LoadingService.DoStep("Применение конфигурации устройств");
							if (FiresecManager.FiresecDriver != null)
							{
								var fsResult = FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
								if (fsResult.HasError)
								{
									MessageBoxService.ShowError(fsResult.Error);
								}
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

					if (ServiceFactory.SaveService.FSChanged ||
						ServiceFactory.SaveService.PlansChanged ||
						ServiceFactory.SaveService.GKChanged)
						FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
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

		public static string SaveAllConfigToFile(bool saveAnyway = false)
		{
			try
			{
				var tempFolderName = AppDataFolderHelper.GetTempFolder();
				if (!Directory.Exists(tempFolderName))
					Directory.CreateDirectory(tempFolderName);

				var tempFileName = AppDataFolderHelper.GetTempFileName();
				if (File.Exists(tempFileName))
					File.Delete(tempFileName);

				TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

				if (ServiceFactory.SaveService.FSChanged || saveAnyway)
					AddConfiguration(tempFolderName, "DeviceConfiguration.xml", FiresecManager.FiresecConfiguration.DeviceConfiguration, 1, 1);
				if (ServiceFactory.SaveService.PlansChanged || saveAnyway)
					AddConfiguration(tempFolderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1);
				if ((ServiceFactory.SaveService.InstructionsChanged) || (ServiceFactory.SaveService.SoundsChanged) || (ServiceFactory.SaveService.FilterChanged) || (ServiceFactory.SaveService.CamerasChanged) || (ServiceFactory.SaveService.EmailsChanged) || saveAnyway)
					AddConfiguration(tempFolderName, "SystemConfiguration.xml", FiresecManager.SystemConfiguration, 1, 1);
				if (ServiceFactory.SaveService.GKChanged || ServiceFactory.SaveService.XInstructionsChanged || saveAnyway)
					AddConfiguration(tempFolderName, "XDeviceConfiguration.xml", XManager.DeviceConfiguration, 1, 1);
				AddConfiguration(tempFolderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1);
				if (ServiceFactory.SaveService.SecurityChanged || saveAnyway)
					AddConfiguration(tempFolderName, "SecurityConfiguration.xml", FiresecManager.SecurityConfiguration, 1, 1);
				if (ServiceFactory.SaveService.LibraryChanged || saveAnyway)
					AddConfiguration(tempFolderName, "DeviceLibraryConfiguration.xml", FiresecManager.DeviceLibraryConfiguration, 1, 1);
				if (ServiceFactory.SaveService.XLibraryChanged || saveAnyway)
					AddConfiguration(tempFolderName, "XDeviceLibraryConfiguration.xml", XManager.XDeviceLibraryConfiguration, 1, 1);

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
			catch (Exception e)
			{
				Logger.Error(e, "ConfigManager.SaveAllConfigToFile");
			}
			return null;
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name));

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		public static void CreateNew()
		{
			try
			{
				var result = MessageBoxService.ShowQuestion("Вы уверены, что хотите создать новую конфигурацию");
				if (result == MessageBoxResult.Yes)
				{
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
					FiresecManager.SetEmptyConfiguration();
					XManager.SetEmptyConfiguration();
					FiresecManager.PlansConfiguration = new PlansConfiguration();
					FiresecManager.SystemConfiguration.Instructions = new List<Instruction>();
					FiresecManager.SystemConfiguration.Cameras = new List<Camera>();
					FiresecManager.PlansConfiguration.Update();

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

					ServiceFactory.SaveService.FSChanged = true;
					ServiceFactory.SaveService.PlansChanged = true;
					ServiceFactory.SaveService.InstructionsChanged = true;
					ServiceFactory.SaveService.CamerasChanged = true;
					ServiceFactory.SaveService.GKChanged = true;
					ServiceFactory.SaveService.XInstructionsChanged = true;

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
			if (ApplicationService.Modules.Any(x => x.Name == "Устройства, Зоны, Направления"))
			{
				var deviceUID = Guid.Empty;
				var firstDevice = FiresecManager.Devices.FirstOrDefault();
				if (firstDevice != null)
					deviceUID = firstDevice.UID;
				ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(deviceUID);
			}
			else if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
			{
				var deviceUID = Guid.Empty;
				var firstDevice = XManager.DeviceConfiguration.Devices.FirstOrDefault();
				if (firstDevice != null)
					deviceUID = firstDevice.UID;
				ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(deviceUID);
			}
		}
	}
}