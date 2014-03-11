using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Common;
using FiresecAPI;
using Ionic.Zip;
using XFiresecAPI;
using System.Windows.Forms;

namespace Infrastructure.Common
{
	public static class XDeviceLibraryConfigurationPatchHelper
	{
		public static void Patch()
		{
			PatchLibrary("XDeviceLibraryConfiguration.xml", typeof(XDeviceLibraryConfiguration));
		}

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
					AddConfigurationToZip(zipFile, configuration, "XDeviceLibraryConfiguration.xml");
					zipFile.Save(fileName);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.PatchLibrary");
			}
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
				Logger.Error(e, "XDeviceLibraryConfigurationPatchHelper.GetConfigurationFomZip " + fileName);
			}
			return null;
		}

		static void AddConfigurationToZip(ZipFile zipFile, VersionedConfiguration versionedConfiguration, string fileName)
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