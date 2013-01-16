using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using Ionic.Zip;
using System.IO;
using Infrastructure.Common;
using FiresecAPI;
using Common;
using System.Runtime.Serialization;

namespace FiresecService.Processor
{
	public static class SecurityConfigurationHelper
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