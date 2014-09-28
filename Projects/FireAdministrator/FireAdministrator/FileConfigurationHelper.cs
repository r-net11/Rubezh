using System;
using System.IO;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using Ionic.Zip;
using Microsoft.Win32;

namespace FireAdministrator
{
	public static class FileConfigurationHelper
	{
		public static string SaveToFile()
		{
			try
			{
				var saveDialog = new SaveFileDialog()
				{
					Filter = "firesec2 files|*.fscp",
					DefaultExt = "firesec2 files|*.fscp"
				};
				if (saveDialog.InitialDirectory != null)
				{
					var fileName = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
					fileName = fileName.Replace(":", " ");
					saveDialog.FileName = Path.Combine(saveDialog.InitialDirectory, fileName + ".fscp");
				}

				if (saveDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						SaveToZipFile(saveDialog.FileName);
					});
					return saveDialog.FileName;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SaveToFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
			return null;
		}

		public static void SaveToZipFile(string fileName)
		{
			var folderName = AppDataFolderHelper.GetTempFolder();
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

			if (File.Exists(fileName))
				File.Delete(fileName);

			TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

			AddConfiguration(folderName, "DeviceConfiguration.xml", FiresecManager.FiresecConfiguration.DeviceConfiguration, 1, 1);
			AddConfiguration(folderName, "PlansConfiguration.xml", FiresecManager.PlansConfiguration, 1, 1);
			AddConfiguration(folderName, "SystemConfiguration.xml", FiresecManager.SystemConfiguration, 1, 1);
			AddConfiguration(folderName, "XDeviceConfiguration.xml", XManager.DeviceConfiguration, 1, 1);
			AddConfiguration(folderName, "SKDConfiguration.xml", SKDManager.SKDConfiguration, 1, 1);
			AddConfiguration(folderName, "LayoutsConfiguration.xml", FiresecManager.LayoutsConfiguration, 1, 1);
			AddConfiguration(folderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1);

			var destinationImagesDirectory = AppDataFolderHelper.GetFolder(Path.Combine(folderName, "Content"));
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

			var zipFile = new ZipFile(fileName);
			zipFile.AddDirectory(folderName);
			zipFile.Save(fileName);
			zipFile.Dispose();
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name), true);

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		public static string LoadFromFile()
		{
			try
			{
				var openDialog = new OpenFileDialog
					{
						Filter = "firesec2 files|*.fscp",
						DefaultExt = "firesec2 files|*.fscp"
					};
				if (openDialog.ShowDialog().Value)
				{
					LoadFromFile(openDialog.FileName);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.LoadFromFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
			return null;
		}

		public static string LoadFromFile(string fileName)
		{
			try
			{
				WaitHelper.Execute(() =>
				{
					ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
					//ZipConfigActualizeHelper.Actualize(fileName, false);
					var folderName = AppDataFolderHelper.GetLocalFolder("Administrator/Configuration");
					var configFileName = Path.Combine(folderName, "Config.fscp");
					if (Directory.Exists(folderName))
						Directory.Delete(folderName, true);
					Directory.CreateDirectory(folderName);
					File.Copy(fileName, configFileName);
					bool isFullConfiguration;
					FiresecManager.LoadFromZipFile(configFileName, out isFullConfiguration);
					ServiceFactory.ContentService.Invalidate();

					FiresecManager.UpdateConfiguration();
					XManager.UpdateConfiguration();
					SKDManager.UpdateConfiguration();

					if (LoadingErrorManager.HasError)
						MessageBoxService.ShowWarning(LoadingErrorManager.ToString(), "Ошибки при загрузке конфигурации");

					ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
					ConfigManager.ShowFirstDevice();

					if (isFullConfiguration)
					{
						ServiceFactory.SaveService.Set();
					}
					else
					{
						ServiceFactory.SaveService.FSChanged = true;
						ServiceFactory.SaveService.PlansChanged = true;
						ServiceFactory.SaveService.GKChanged = true;
						ServiceFactory.SaveService.SKDChanged = true;
						ServiceFactory.SaveService.LayoutsChanged = true;
					}
				});
				return fileName;
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.LoadFromFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
			return null;
		}

		public static void SaveAllToFile()
		{
			try
			{
				var saveDialog = new SaveFileDialog()
				{
					Filter = "firesec2 files|*.fscp",
					DefaultExt = "firesec2 files|*.fscp"
				};
				if (saveDialog.InitialDirectory != null)
				{
					var fileName = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString();
					fileName = fileName.Replace(":", " ");
					saveDialog.FileName = Path.Combine(saveDialog.InitialDirectory, fileName + ".fscp");
				}

				if (saveDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						var tempFileName = ConfigManager.SaveAllConfigToFile();
						File.Copy(tempFileName, saveDialog.FileName);
						File.Delete(tempFileName);
					});
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SaveToFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}
	}
}