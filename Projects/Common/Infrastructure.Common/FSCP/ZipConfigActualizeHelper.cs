using System;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using Ionic.Zip;
using XFiresecAPI;

namespace Infrastructure.Common
{
	public static class ZipConfigActualizeHelper
	{
		public static void Actualize(string fileName, bool isFull = false)
		{
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			bool result;

			var plansConfiguration = GetConfigurationFromZip<PlansConfiguration>(zipFile, "PlansConfiguration.xml", out result);
			if (!result)
			{
				AddConfigurationToZip(zipFile, plansConfiguration, "PlansConfiguration.xml");
			}
			var systemConfiguration = GetConfigurationFromZip<SystemConfiguration>(zipFile, "SystemConfiguration.xml", out result);
			if (!result)
			{
				AddConfigurationToZip(zipFile, systemConfiguration, "SystemConfiguration.xml");
			}
			var deviceConfiguration = GetConfigurationFromZip<DeviceConfiguration>(zipFile, "DeviceConfiguration.xml", out result);
			if (!result)
			{
				AddConfigurationToZip(zipFile, deviceConfiguration, "DeviceConfiguration.xml");
			}
			var xDeviceConfiguration = GetConfigurationFromZip<XDeviceConfiguration>(zipFile, "XDeviceConfiguration.xml", out result);
			if (!result)
			{
				AddConfigurationToZip(zipFile, xDeviceConfiguration, "XDeviceConfiguration.xml");
			}

			if (isFull)
			{
				var securityConfiguration = GetConfigurationFromZip<SecurityConfiguration>(zipFile, "SecurityConfiguration.xml", out result);
				if (!result)
				{
					AddConfigurationToZip(zipFile, securityConfiguration, "SecurityConfiguration.xml");
				}
				var driversConfiguration = GetConfigurationFromZip<DriversConfiguration>(zipFile, "DriversConfiguration.xml", out result);
				if (!result)
				{
					AddConfigurationToZip(zipFile, driversConfiguration, "DriversConfiguration.xml");
				}
				var deviceLibraryConfiguration = GetConfigurationFromZip<DeviceLibraryConfiguration>(zipFile, "DeviceLibraryConfiguration.xml", out result);
				if (!result)
				{
					AddConfigurationToZip(zipFile, deviceLibraryConfiguration, "DeviceLibraryConfiguration.xml");
				}
				var xDeviceLibraryConfiguration = GetConfigurationFromZip<XDeviceLibraryConfiguration>(zipFile, "XDeviceLibraryConfiguration.xml", out result);
				if (!result)
				{
					AddConfigurationToZip(zipFile, xDeviceLibraryConfiguration, "XDeviceLibraryConfiguration.xml");
				}
			}

			var zipConfigurationItemsCollection = GetConfigurationFromZip<ZipConfigurationItemsCollection>(zipFile, "ZipConfigurationItemsCollection.xml", out result);
			if (!result)
			{
				zipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("SecurityConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("SystemConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("PlansConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("DriversConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("DeviceConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("DeviceLibraryConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("XDeviceConfiguration", 1, 1));
				zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("XDeviceLibraryConfiguration", 1, 1));
				AddConfigurationToZip(zipFile, zipConfigurationItemsCollection, "ZipConfigurationItemsCollection.xml");
			}

			if (zipFile.Entries.Any(x => x.FileName == "Info.xml"))
				zipFile.RemoveEntry("Info.xml");

			zipFile.Save(fileName);
		}

		static T GetConfigurationFromZip<T>(ZipFile zipFile, string fileName, out bool result)
		where T : VersionedConfiguration, new()
		{
			result = true;
			try
			{
				var configurationEntry = zipFile[fileName];
				if (configurationEntry != null)
				{
					var configurationMemoryStream = new MemoryStream();
					configurationEntry.Extract(configurationMemoryStream);
					configurationMemoryStream.Position = 0;

					T configuration = ZipSerializeHelper.DeSerialize<T>(configurationMemoryStream);
					if (!configuration.ValidateVersion())
					{
						result = false;
					}
					return configuration;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
			}
			T newConfiguration = new T();
			result = false;
			return newConfiguration;
		}

		static void AddConfigurationToZip(ZipFile zipFile, VersionedConfiguration versionedConfiguration, string fileName)
		{
			var configuarationMemoryStream = ZipSerializeHelper.Serialize(versionedConfiguration);
			if (zipFile.Entries.Any(x => x.FileName == fileName))
				zipFile.RemoveEntry(fileName);
			configuarationMemoryStream.Position = 0;
			zipFile.AddEntry(fileName, configuarationMemoryStream);
		}
	}
}