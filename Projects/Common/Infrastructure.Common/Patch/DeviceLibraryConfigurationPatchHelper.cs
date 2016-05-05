using Common;
using StrazhAPI;
using StrazhAPI.SKD;
using Ionic.Zip;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Infrastructure.Common
{
	public static class DeviceLibraryConfigurationPatchHelper
	{
		public static void PatchSKDLibrary()
		{
			PatchLibrary("SKDDeviceLibraryConfiguration.xml", typeof(SKDLibraryConfiguration));
		}

		public static void PatchLibrary(string configurationFileName, Type configurationType)
		{
			try
			{
				var emptyFileName = AppDataFolderHelper.GetFileInFolder("Empty", "Config.fscp");
				var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");

				var emptyZipFile = ZipFile.Read(emptyFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
				var configuration = GetConfigurationFomZip(emptyZipFile, configurationFileName, configurationType);

				if (configuration != null)
				{
					var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
					//	AddConfigurationToZip(zipFile, configuration, "GKDeviceLibraryConfiguration.xml");
					zipFile.Save(fileName);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.PatchLibrary");
			}
		}

		private static VersionedConfiguration GetConfigurationFomZip(ZipFile zipFile, string fileName, Type type)
		{
			try
			{
				var configurationEntry = zipFile[fileName];
				if (configurationEntry != null)
				{
					var configurationMemoryStream = new MemoryStream();
					configurationEntry.Extract(configurationMemoryStream);
					configurationMemoryStream.Position = 0;

					var xmlSerializer = XmlSerializer.FromTypes(new[] {type})[0]; //new XmlSerializer(type);
					return (VersionedConfiguration)xmlSerializer.Deserialize(configurationMemoryStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "GKDeviceLibraryConfigurationPatchHelper.GetConfigurationFomZip " + fileName);
			}
			return null;
		}

		private static void AddConfigurationToZip(ZipFile zipFile, VersionedConfiguration versionedConfiguration, string fileName)
		{
			var configuarationMemoryStream = ZipSerializeHelper.Serialize(versionedConfiguration);
			if (zipFile.Entries.Any(x => x.FileName == fileName))
			{
				zipFile.RemoveEntry(fileName);
			}
			configuarationMemoryStream.Position = 0;
			zipFile.AddEntry(fileName, configuarationMemoryStream);
		}
	}
}