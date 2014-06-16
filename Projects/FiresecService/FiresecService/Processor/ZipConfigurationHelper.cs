using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Ionic.Zip;

namespace FiresecService.Processor
{
	public static class ZipConfigurationHelper
	{
		public static SecurityConfiguration GetSecurityConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var securityConfiguration = (SecurityConfiguration)GetConfigurationFomZip(zipFile, "SecurityConfiguration.xml", typeof(SecurityConfiguration));
			securityConfiguration.AfterLoad();
			zipFile.Dispose();
			return securityConfiguration;
		}

		public static SystemConfiguration GetSystemConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			if (zipFile != null)
			{
				var systemConfiguration = (SystemConfiguration)GetConfigurationFomZip(zipFile, "SystemConfiguration.xml", typeof(SystemConfiguration));
				if (systemConfiguration != null)
				{
					systemConfiguration.AfterLoad();
				}
				zipFile.Dispose();
				return systemConfiguration;
			}
			return new SystemConfiguration();
		}

		public static XDeviceConfiguration GetDeviceConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var deviceConfiguration = (XDeviceConfiguration)GetConfigurationFomZip(zipFile, "XDeviceConfiguration.xml", typeof(XDeviceConfiguration));
			if (deviceConfiguration == null)
				deviceConfiguration = new XDeviceConfiguration();
			deviceConfiguration.AfterLoad();
			zipFile.Dispose();
			return deviceConfiguration;
		}

		public static SKDConfiguration GetSKDConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			if (zipFile != null)
			{
				var skdConfiguration = (SKDConfiguration)GetConfigurationFomZip(zipFile, "SKDConfiguration.xml", typeof(SKDConfiguration));
				if (skdConfiguration != null)
				{
					skdConfiguration.AfterLoad();
				}
				zipFile.Dispose();
				return skdConfiguration;
			}
			return new SKDConfiguration();
		}

		static VersionedConfiguration GetConfigurationFomZip(ZipFile zipFile, string fileName, Type type)
		{
			try
			{
				var configurationEntry = zipFile[fileName];
				if (configurationEntry != null)
				{
					var configurationMemoryStream = new MemoryStream();
					configurationEntry.Extract(configurationMemoryStream);
					configurationMemoryStream.Position = 0;

					var dataContractSerializer = new DataContractSerializer(type);
					return (VersionedConfiguration)dataContractSerializer.ReadObject(configurationMemoryStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
			}
			return null;
		}
	}
}