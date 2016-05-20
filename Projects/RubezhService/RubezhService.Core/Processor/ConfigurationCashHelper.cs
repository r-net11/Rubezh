using Common;
using GKProcessor;
using Infrastructure.Common;
using Ionic.Zip;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using System;
using System.IO;
using System.Xml.Serialization;

namespace RubezhService
{
	public static class ConfigurationCashHelper
	{
		public static SecurityConfiguration SecurityConfiguration { get; private set; }
		public static SystemConfiguration SystemConfiguration { get; private set; }

		public static void Update()
		{
			CheckConfigDirectory();

			var result = GetSecurityConfiguration();
			if (!result.HasError && result.Result != null)
				SecurityConfiguration = result.Result;
			SystemConfiguration = GetSystemConfiguration();
			if (SystemConfiguration == null)
				SystemConfiguration = new SystemConfiguration();

			GKManager.DeviceConfiguration = GetDeviceConfiguration();

			SystemConfiguration.UpdateConfiguration();

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

		public static OperationResult<SecurityConfiguration> GetSecurityConfiguration()
		{
			string error = "";
			var securityConfiguration = new SecurityConfiguration();
			try
			{
				var configDirectory = AppDataFolderHelper.GetServerAppDataPath("Config");
				if (File.Exists(Path.Combine(configDirectory, "SecurityConfiguration.xml")))
				{
					if (!File.Exists(AppDataFolderHelper.GetServerAppDataPath("Config" + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "SecurityConfiguration.xml")))
						File.Copy(Path.Combine(configDirectory, "SecurityConfiguration.xml"), AppDataFolderHelper.GetServerAppDataPath("Config" + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "SecurityConfiguration.xml"));
					File.Delete(Path.Combine(configDirectory, "SecurityConfiguration.xml"));
				}
				securityConfiguration = (SecurityConfiguration)GetConfiguration("SecurityConfiguration.xml", typeof(SecurityConfiguration));
				securityConfiguration.AfterLoad();
			}
			catch (Exception ex)
			{
				error = ex.Message;
			}
			return OperationResult<SecurityConfiguration>.FromError(error, securityConfiguration);
		}

		static SystemConfiguration GetSystemConfiguration()
		{
			var systemConfiguration = (SystemConfiguration)GetConfiguration("Config" + Path.DirectorySeparatorChar + "SystemConfiguration.xml", typeof(SystemConfiguration));
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
			var deviceConfiguration = (GKDeviceConfiguration)GetConfiguration("Config" + Path.DirectorySeparatorChar + "GKDeviceConfiguration.xml", typeof(GKDeviceConfiguration));
			if (deviceConfiguration == null)
				deviceConfiguration = new GKDeviceConfiguration();
			deviceConfiguration.AfterLoad();
			return deviceConfiguration;
		}

		static VersionedConfiguration GetConfiguration(string fileName, Type type)
		{
			try
			{
				var configDirectoryName = AppDataFolderHelper.GetServerAppDataPath();
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
				Logger.Error(e, "ConfigurationCashHelper.GetConfiguration " + fileName);
			}
			return null;
		}
	}
}