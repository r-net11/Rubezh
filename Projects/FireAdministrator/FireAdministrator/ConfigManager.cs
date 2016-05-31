using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using Common;
using FireAdministrator.Properties;
using StrazhAPI;
using StrazhAPI.Automation;
using StrazhAPI.Models;
using StrazhAPI.SKD;
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
#if DEBUG
				Logger.Info("Начата процедура изменения конфигурации");
#endif
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
				//	FiresecManager.FiresecService.GKAddMessage(JournalEventNameType.Применение_конфигурации, "");
				LoadingService.Show("Применение конфигурации", "Применение конфигурации", 1);

				if (ConnectionSettingsManager.IsRemote)
				{
					var tempFileName = SaveConfigToFile(false);
					LoadingService.DoStep(null);
					using (var fileStream = new FileStream(tempFileName, FileMode.Open))
					{
						FiresecManager.FiresecService.SetConfig(fileStream);
					}
					File.Delete(tempFileName);
				}
				else
				{
					SaveConfigToFile(true);
					LoadingService.DoStep(null);
					FiresecManager.FiresecService.SetLocalConfig();
				}

				if (ServiceFactory.SaveService.HasChanges)
#if DEBUG
					Logger.Info("Рассылаем уведомление клиентам об изменении конфигурации");
#endif
					FiresecManager.FiresecService.NotifyClientsOnConfigurationChanged();
				ServiceFactory.SaveService.Reset();
				LoadingService.Close();
#if DEBUG
				Logger.Info("Закончена процедура изменения конфигурации");
#endif
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
				var tempFolderName = isLocal ? AppDataFolderHelper.GetServerAppDataPath("Config") : AppDataFolderHelper.GetTempFolder();

				if (!Directory.Exists(tempFolderName))
					Directory.CreateDirectory(tempFolderName);

				TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

				if (ServiceFactory.SaveService.PlansChanged)
					AddConfiguration(tempFolderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.SoundsChanged || ServiceFactory.SaveService.FilterChanged || ServiceFactory.SaveService.CamerasChanged || ServiceFactory.SaveService.EmailsChanged || ServiceFactory.SaveService.AutomationChanged)
					AddConfiguration(tempFolderName, "SystemConfiguration.xml", FiresecManager.SystemConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.SecurityChanged)
					AddConfiguration(tempFolderName, "SecurityConfiguration.xml", FiresecManager.SecurityConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.SKDChanged)
					AddConfiguration(tempFolderName, "SKDConfiguration.xml", SKDManager.SKDConfiguration, 1, 1, true);
				if (ServiceFactory.SaveService.SKDLibraryChanged)
					AddConfiguration(tempFolderName, "SKDLibraryConfiguration.xml", SKDManager.SKDLibraryConfiguration, 1, 1, true);
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
				if (MessageBoxService.ShowQuestion(Resources.CreateNewConfigurationMessage))
				{
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
					FiresecManager.PlansConfiguration = new PlansConfiguration();
					FiresecManager.SystemConfiguration.Cameras = new List<Camera>();
					FiresecManager.SystemConfiguration.AutomationConfiguration = new AutomationConfiguration();
					FiresecManager.PlansConfiguration.Update();
					SKDManager.SetEmptyConfiguration();
					FiresecManager.LayoutsConfiguration = new LayoutsConfiguration();

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);

					ServiceFactory.SaveService.PlansChanged = true;
					ServiceFactory.SaveService.CamerasChanged = true;
					ServiceFactory.SaveService.SKDChanged = true;
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
		}
	}
}