using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using XFiresecAPI;
using Infrastructure.Common;
using System.Collections.Generic;
using Ionic.Zip;
using System.Text;

namespace FiresecService
{
	public static class PatchManager
	{
		public static void Patch()
		{
			try
			{
				Patch2();
			}
			catch(Exception e)
			{
				Logger.Error(e, "PatchManager.Patch");
			}
		}

		static void Patch2()
		{
			var patchNo = PatchHelper.GetPatchNo("Server");
			if (patchNo >= 5)
				return;

			var emptyFileName = AppDataFolderHelper.GetFileInFolder("Empty", "Config.fscp");
			var fileName = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");

			var emptyZipFile = ZipFile.Read(emptyFileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var deviceLibraryConfiguration = GetConfigurationFomZip(emptyZipFile, "DeviceLibraryConfiguration.xml", typeof(DeviceLibraryConfiguration));
			var xDeviceLibraryConfiguration = GetConfigurationFomZip(emptyZipFile, "XDeviceLibraryConfiguration.xml", typeof(XDeviceLibraryConfiguration));
			var driversConfiguration = GetConfigurationFomZip(emptyZipFile, "DriversConfiguration.xml", typeof(DriversConfiguration));

			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			AddConfigurationToZip(zipFile, deviceLibraryConfiguration, "DeviceLibraryConfiguration.xml");
			AddConfigurationToZip(zipFile, xDeviceLibraryConfiguration, "XDeviceLibraryConfiguration.xml");
			AddConfigurationToZip(zipFile, driversConfiguration, "DriversConfiguration.xml");
			zipFile.Save(fileName);

			PatchHelper.SetPatchNo("Server", 5);
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