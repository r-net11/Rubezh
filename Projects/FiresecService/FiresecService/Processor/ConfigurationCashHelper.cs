using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.Automation;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
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

			GKManager.DeviceConfiguration = GetDeviceConfiguration();
			SKDManager.SKDConfiguration = GetSKDConfiguration();

			SystemConfiguration.UpdateConfiguration();

			GKDriversCreator.Create();
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
			DescriptorsManager.CreateDynamicObjectsInGKManager();
			GKManager.UpdateConfiguration();

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
			var securityConfiguration = (SecurityConfiguration)GetConfigurationFomZip("SecurityConfiguration.xml", typeof(SecurityConfiguration));
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

		static GKDeviceConfiguration GetDeviceConfiguration()
		{
			var deviceConfiguration = (GKDeviceConfiguration)GetConfigurationFomZip("GKDeviceConfiguration.xml", typeof(GKDeviceConfiguration));
			if (deviceConfiguration == null)
				deviceConfiguration = new GKDeviceConfiguration();
			deviceConfiguration.AfterLoad();
			return deviceConfiguration;
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

				var stream = new FileStream(filePath, FileMode.Open);
				var xmlSerializer = new XmlSerializer(type);
				var versionedConfiguration = (VersionedConfiguration)xmlSerializer.Deserialize(stream);
				stream.Close();
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