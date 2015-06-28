using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using FiresecService.Properties;
using GKProcessor;
using Infrastructure.Common;
using Ionic.Zip;
using System.Xml.Serialization;

namespace FiresecService
{
	public static class ConfigurationCashHelper
	{
		public static SecurityConfiguration SecurityConfiguration { get; private set; }
		public static SystemConfiguration SystemConfiguration { get; private set; }

		public static void Update()
		{
			CheckConfigDirectory();

			SecurityConfiguration = GetSecurityConfiguration();
			SystemConfiguration = GetSystemConfiguration();
			if (SystemConfiguration == null)
				SystemConfiguration = new SystemConfiguration();

			SKDManager.SKDConfiguration = GetSKDConfiguration();

			SystemConfiguration.UpdateConfiguration();

			SKDManager.UpdateConfiguration();
		}

		static void CheckConfigDirectory()
		{
			var configFileName = AppDataFolderHelper.GetServerAppDataPath("Config.fscp");
			var configDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");
			var contetntDirectory = Path.Combine(configDirectory, "Content");
			if (!Directory.Exists(configDirectory))
			{
				Directory.CreateDirectory(configDirectory);
				var zipFile = new ZipFile(configFileName);
				zipFile.ExtractAll(configDirectory);
			}
			if (!Directory.Exists(contetntDirectory))
			{
				Directory.CreateDirectory(contetntDirectory);
			}
		}

		public static SecurityConfiguration GetSecurityConfiguration()
		{
			var securityConfiguration = GetConfigurationFomZip("SecurityConfiguration.xml", typeof(SecurityConfiguration)) as SecurityConfiguration;

			if (securityConfiguration == null)
			{
				MessageBox.Show(Resources.FileNotExistError);
				Application.Current.MainWindow.Close();
			}

			securityConfiguration.AfterLoad();

			return securityConfiguration;
		}

		static SystemConfiguration GetSystemConfiguration()
		{
			var systemConfiguration = (SystemConfiguration)GetConfigurationFomZip("SystemConfiguration.xml", typeof(SystemConfiguration));
			if (systemConfiguration != null)
			{
				systemConfiguration.AfterLoad();
			}
			else
			{
				systemConfiguration = new SystemConfiguration();
			}
			return systemConfiguration;
		}

		static SKDConfiguration GetSKDConfiguration()
		{
			var skdConfiguration = (SKDConfiguration)GetConfigurationFomZip("SKDConfiguration.xml", typeof(SKDConfiguration));
			if (skdConfiguration != null)
			{
				skdConfiguration.AfterLoad();
			}
			else
			{
				skdConfiguration = new SKDConfiguration();
			}
			return skdConfiguration;
		}

		static VersionedConfiguration GetConfigurationFomZip(string fileName, Type type)
		{
			try
			{
				var configDirectoryName = AppDataFolderHelper.GetServerAppDataPath("Config");
				var filePath = Path.Combine(configDirectoryName, fileName);

				VersionedConfiguration versionedConfiguration;

				if (!File.Exists(filePath))
					return new VersionedConfiguration();

				using (Stream stream = new FileStream(filePath, FileMode.Open))
				{
					var xmlSerializer = new XmlSerializer(type);
					versionedConfiguration = (VersionedConfiguration)xmlSerializer.Deserialize(stream);

				}

				versionedConfiguration.ValidateVersion();
				return versionedConfiguration;
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
			}
			return null;
		}
	}
}