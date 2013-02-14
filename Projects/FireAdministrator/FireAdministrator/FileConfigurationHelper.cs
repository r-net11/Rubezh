using System;
using System.IO;
using System.Linq;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;
using Ionic.Zip;
using Microsoft.Win32;
using Infrastructure.Common.Windows;

namespace FireAdministrator
{
	public static class FileConfigurationHelper
	{
		public static void SaveToFile()
		{
			try
			{
				var saveDialog = new SaveFileDialog()
				{
					Filter = "firesec2 files|*.fscp",
					DefaultExt = "firesec2 files|*.fscp"
				};
				if (saveDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						SaveToZipFile(saveDialog.FileName);
					});
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SaveToFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
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
			AddConfiguration(folderName, "ZipConfigurationItemsCollection.xml", TempZipConfigurationItemsCollection, 1, 1);

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
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name));

			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		public static void LoadFromFile()
		{
			try
			{
				var openDialog = new OpenFileDialog()
					{
						Filter = "firesec2 files|*.fscp",
						DefaultExt = "firesec2 files|*.fscp"
					};
				if (openDialog.ShowDialog().Value)
				{
					WaitHelper.Execute(() =>
					{
						ServiceFactory.Events.GetEvent<ConfigurationClosedEvent>().Publish(null);
						ZipConfigActualizeHelper.Actualize(openDialog.FileName, false);
						var folderName = AppDataFolderHelper.GetFolder("Administrator/Configuration");
						var configFileName = Path.Combine(folderName, "Config.fscp");
						if (Directory.Exists(folderName))
							Directory.Delete(folderName, true);
						Directory.CreateDirectory(folderName);
						File.Copy(openDialog.FileName, configFileName);
						FiresecManager.LoadFromZipFile(configFileName);

						FiresecManager.UpdateConfiguration();
						XManager.UpdateConfiguration();

						ServiceFactory.Events.GetEvent<ConfigurationChangedEvent>().Publish(null);
						ServiceFactory.Layout.Close();
						if (ApplicationService.Modules.Any(x => x.Name == "Устройства, Зоны, Направления"))
							ServiceFactory.Events.GetEvent<ShowDeviceEvent>().Publish(Guid.Empty);
						if (ApplicationService.Modules.Any(x => x.Name == "Групповой контроллер"))
							ServiceFactory.Events.GetEvent<ShowXDeviceEvent>().Publish(Guid.Empty);

						ServiceFactory.SaveService.FSChanged = true;
						ServiceFactory.SaveService.PlansChanged = true;
						ServiceFactory.SaveService.GKChanged = true;
						ServiceFactory.Layout.ShowFooter(null);
					});
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.LoadFromFile");
				MessageBox.Show(e.Message, "Ошибка при выполнении операции");
			}
		}
	}
}