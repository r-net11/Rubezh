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
						LoadingService.DoStep("Применение конфигурации устройств");
						if (!GlobalSettingsHelper.GlobalSettings.DoNotOverrideFS1)
						{
							if (FiresecManager.FiresecDriver != null)
							{
								var fsResult = FiresecManager.FiresecDriver.SetNewConfig(FiresecManager.FiresecConfiguration.DeviceConfiguration);
								if (fsResult.HasError)
								{
									MessageBoxService.ShowError(fsResult.Error);
								}
							}
						}
					}

					var tempFolderName = AppDataFolderHelper.GetTempFolder();
					if (!Directory.Exists(tempFolderName))
						Directory.CreateDirectory(tempFolderName);

					var tempFileName = AppDataFolderHelper.GetTempFileName();
					if (File.Exists(tempFileName))
						File.Delete(tempFileName);

					TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

					if (ServiceFactory.SaveService.FSChanged)
						AddConfiguration(tempFolderName, "DeviceConfiguration.xml", FiresecManager.FiresecConfiguration.DeviceConfiguration, 1, 1);
					if (ServiceFactory.SaveService.PlansChanged)
						AddConfiguration(tempFolderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1);
					if ((ServiceFactory.SaveService.InstructionsChanged) || (ServiceFactory.SaveService.SoundsChanged) || (ServiceFactory.SaveService.FilterChanged) || (ServiceFactory.SaveService.CamerasChanged) || (ServiceFactory.SaveService.EmailsChanged))
						AddConfiguration(tempFolderName, "SystemConfiguration.xml", FiresecManager.SystemConfiguration, 1, 1);
					if (ServiceFactory.SaveService.GKChanged || ServiceFactory.SaveService.XInstructionsChanged)
						AddConfiguration(tempFolderName, "XDeviceConfiguration.xml", XManager.DeviceConfiguration, 1, 1);
					AddConfiguration(tempFolderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1);
					if (ServiceFactory.SaveService.SecurityChanged)
						AddConfiguration(tempFolderName, "SecurityConfiguration.xml", FiresecManager.SecurityConfiguration, 1, 1);
					if (ServiceFactory.SaveService.LibraryChanged)
						AddConfiguration(tempFolderName, "DeviceLibraryConfiguration.xml", FiresecManager.DeviceLibraryConfiguration, 1, 1);
					if (ServiceFactory.SaveService.XLibraryChanged)
						AddConfiguration(tempFolderName, "XDeviceLibraryConfiguration.xml", XManager.XDeviceLibraryConfiguration, 1, 1);

					var sourceImagesDirectory = AppDataFolderHelper.GetFolder("Administrator/Configuration/Unzip/Images");
					var destinationImagesDirectory = AppDataFolderHelper.GetFolder(Path.Combine(tempFolderName, "Images"));
					if (Directory.Exists(sourceImagesDirectory))
					{
						if (Directory.Exists(destinationImagesDirectory))
							Directory.Delete(destinationImagesDirectory);
						if (!Directory.Exists(destinationImagesDirectory))
							Directory.CreateDirectory(destinationImagesDirectory);
						var sourceImagesDirectoryInfo = new DirectoryInfo(sourceImagesDirectory);
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

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		private static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
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

					ServiceFactory.Layout.Close();
					if (ApplicationService.Modules.Any(x => x.Name == "Устройства, Зоны, Направления"))
						ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
					if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
						ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Guid.Empty);
					ServiceFactory.Layout.ShowFooter(null);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.CreateNew");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}
	}
}