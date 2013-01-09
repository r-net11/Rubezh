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
			if (File.Exists(fileName))
				File.Delete(fileName);
			var zipFile = new ZipFile(fileName);

			TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();

			AddConfiguration(zipFile, FiresecManager.FiresecConfiguration.DeviceConfiguration, "DeviceConfiguration.xml", 1, 1);
			AddConfiguration(zipFile, FiresecManager.PlansConfiguration, "PlansConfiguration.xml", 1, 1);
			AddConfiguration(zipFile, FiresecManager.SystemConfiguration, "SystemConfiguration.xml", 1, 1);
			AddConfiguration(zipFile, XManager.DeviceConfiguration, "XDeviceConfiguration.xml", 1, 1);
			AddConfiguration(zipFile, TempZipConfigurationItemsCollection, "ZipConfigurationItemsCollection.xml", 1, 1);

			zipFile.Save(fileName);
			zipFile.Dispose();
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
					var memoryStream = new FileStream(openDialog.FileName, FileMode.Open, FileAccess.Read);
					memoryStream.Close();
					WaitHelper.Execute(() =>
						{
							ZipConfigActualizeHelper.Actualize(openDialog.FileName, false);
							FiresecManager.LoadFromZipFile(openDialog.FileName);
						});
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