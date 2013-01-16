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

namespace FiresecService.Configuration
{
	public static class PatchManager
	{
		static string LocalConfigurationDirectory(string fileName)
		{
			return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Configuration", fileName);
		}

		public static void Patch()
		{
			var fileName = LocalConfigurationDirectory("config.fscp");
			if (File.Exists(fileName))
			{
				var configurationNames = new Dictionary<string, Type>();
				configurationNames.Add("SecurityConfiguration.xml", typeof(SecurityConfiguration));
				configurationNames.Add("SystemConfiguration.xml", typeof(SystemConfiguration));
				configurationNames.Add("PlansConfiguration.xml", typeof(PlansConfiguration));
				configurationNames.Add("DriversConfiguration.xml", typeof(DriversConfiguration));
				configurationNames.Add("DeviceConfiguration.xml", typeof(DeviceConfiguration));
				configurationNames.Add("DeviceLibraryConfiguration.xml", typeof(DeviceLibraryConfiguration));
				configurationNames.Add("XDeviceConfiguration.xml", typeof(XDeviceConfiguration));
				configurationNames.Add("XDeviceLibraryConfiguration.xml", typeof(XDeviceLibraryConfiguration));

				var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

				foreach (var configurationName in configurationNames)
				{
					var configuration = GetConfigurationFomZip(zipFile, configurationName.Key, configurationName.Value);
					if (configuration == null)
					{
						configuration = GetConfigurationFromDisk(configurationName.Key, configurationName.Value);
						AddConfigurationToZip(zipFile, configuration, configurationName.Key);
					}
					if (File.Exists(LocalConfigurationDirectory(configurationName.Key)))
						File.Delete(LocalConfigurationDirectory(configurationName.Key));
				}

				var zipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("SecurityConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("SystemConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("PlansConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("DriversConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("DeviceConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("DeviceLibraryConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("XDeviceConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("XDeviceLibraryConfiguration", 1, 1));
				AddConfigurationToZip(zipFile, zipConfigurationItemsCollection, "ZipConfigurationItemsCollection.xml");

				zipFile.Save(fileName);

				var appDataConfigFile = Path.Combine(AppDataFolderHelper.GetServerAppDataPath(), "Config.fscp");
				if (File.Exists(fileName) && !File.Exists(appDataConfigFile))
				{
					File.Move(fileName, appDataConfigFile);
				}
				if (File.Exists(fileName))
				{
					File.Delete(fileName);
				}
			}

			if (Directory.Exists("Configuration"))
			{
				Directory.Delete("Configuration", true);
			}
			if (Directory.Exists("Logs"))
			{
				Directory.Delete("Logs", true);
			}
			var localFiresecDBFileName = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Firesec.sdf");
			var appDatalFiresecDBFileName = AppDataFolderHelper.GetDBFile("Firesec.sdf");
			if (File.Exists(localFiresecDBFileName))
			{
				if (File.Exists(appDatalFiresecDBFileName))
				{
					File.Delete(appDatalFiresecDBFileName);
				}
				File.Move(localFiresecDBFileName, appDatalFiresecDBFileName);
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
				Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
			}
			return null;
		}

		static void AddConfigurationToZip(ZipFile zipFile, VersionedConfiguration versionedConfiguration, string fileName)
		{
			var configuarationMemoryStream = ZipSerializeHelper.Serialize(versionedConfiguration);
			if (!zipFile.Entries.Any(x => x.FileName == fileName))
			{
				configuarationMemoryStream.Position = 0;
				zipFile.AddEntry(fileName, configuarationMemoryStream);
			}
		}

		static VersionedConfiguration GetConfigurationFromDisk(string fileName, Type type)
		{
			try
			{
				var fullFileName = LocalConfigurationDirectory(fileName);
				if (File.Exists(fullFileName))
				{
					var memoryStream = new MemoryStream();
					using (var fileStream = new FileStream(fullFileName, FileMode.Open))
					{
						memoryStream.SetLength(fileStream.Length);
						fileStream.Read(memoryStream.GetBuffer(), 0, (int)fileStream.Length);
					}

					var dataContractSerializer = new DataContractSerializer(type);
					var configuration = (VersionedConfiguration)dataContractSerializer.ReadObject(memoryStream);
					return configuration;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigurationFileManager.Get<T> typeof(T) = " + type.Name.ToString());
			}
			VersionedConfiguration newConfiguration = (VersionedConfiguration)Activator.CreateInstance(type);
			return newConfiguration;
		}
	}
}