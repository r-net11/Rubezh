using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using System.IO;
using Ionic.Zip;
using Infrastructure.Common.Windows;
using FiresecAPI;
using System.Runtime.Serialization;
using Common;

namespace GKImitator
{
	public static class ZipConfigurationHelper
	{
		public static XDeviceConfiguration GetDeviceConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var deviceConfiguration = (XDeviceConfiguration)GetConfigurationFomZip(zipFile, "XDeviceConfiguration.xml", typeof(XDeviceConfiguration));
			deviceConfiguration.AfterLoad();
			zipFile.Dispose();
			return deviceConfiguration;
		}

		public static SKDConfiguration GetSKDConfiguration()
		{
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var skdConfiguration = (SKDConfiguration)GetConfigurationFomZip(zipFile, "SKDConfiguration.xml", typeof(SKDConfiguration));
			skdConfiguration.AfterLoad();
			zipFile.Dispose();
			return skdConfiguration;
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