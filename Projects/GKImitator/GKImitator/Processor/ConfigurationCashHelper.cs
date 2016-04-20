using System;
using System.IO;
using System.Xml.Serialization;
using Common;
using RubezhAPI;
using RubezhAPI.GK;
using GKProcessor;
using Infrastructure.Common.Windows;
using Ionic.Zip;

namespace GKImitator
{
	public static class ConfigurationCashHelper
	{
		public static void Update()
		{
			CheckConfigDirectory();
			GKManager.DeviceConfiguration = GetDeviceConfiguration();
			GKDriversCreator.Create();
			GKManager.UpdateConfiguration();
			GKManager.CreateStates();
			DescriptorsManager.Create();
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

		static GKDeviceConfiguration GetDeviceConfiguration()
		{
			var deviceConfiguration = (GKDeviceConfiguration)GetConfiguration("GKDeviceConfiguration.xml", typeof(GKDeviceConfiguration));
			if (deviceConfiguration == null)
				deviceConfiguration = new GKDeviceConfiguration();
			deviceConfiguration.AfterLoad();
			return deviceConfiguration;
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