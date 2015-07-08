﻿using System;
using System.IO;
using System.Xml.Serialization;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using FiresecClient;
using GKProcessor;
using Infrastructure.Common;
using Ionic.Zip;

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

			SystemConfiguration.UpdateConfiguration();

			GKDriversCreator.Create();
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
			DescriptorsManager.CreateDynamicObjectsInGKManager();
			GKManager.UpdateConfiguration();

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
			var securityConfiguration = (SecurityConfiguration)GetConfiguration("SecurityConfiguration.xml", typeof(SecurityConfiguration));
			securityConfiguration.AfterLoad();
			return securityConfiguration;
		}

		static SystemConfiguration GetSystemConfiguration()
		{
			var systemConfiguration = (SystemConfiguration)GetConfiguration("SystemConfiguration.xml", typeof(SystemConfiguration));
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
			var deviceConfiguration = (GKDeviceConfiguration)GetConfiguration("GKDeviceConfiguration.xml", typeof(GKDeviceConfiguration));
			if (deviceConfiguration == null)
				deviceConfiguration = new GKDeviceConfiguration();
			deviceConfiguration.AfterLoad();
			return deviceConfiguration;
		}

		static SKDConfiguration GetSKDConfiguration()
		{
			var skdConfiguration = (SKDConfiguration)GetConfiguration("SKDConfiguration.xml", typeof(SKDConfiguration));
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

		static VersionedConfiguration GetConfiguration(string fileName, Type type)
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