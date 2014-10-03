using System;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Ionic.Zip;

namespace Infrastructure.Common
{
	public static class ZipConfigActualizeHelper
	{
		public static void Actualize(string fileName, bool isFull = false)
		{
			try
			{
				var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
				bool result;

				//var plansConfiguration = GetConfigurationFromZip<PlansConfiguration>(zipFile, "PlansConfiguration.xml", out result);
				//if (!result)
				//{
				//	AddConfigurationToZip(zipFile, plansConfiguration, "PlansConfiguration.xml");
				//}
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
				var gkDeviceConfiguration = GetConfigurationFromZip<GKDeviceConfiguration>(zipFile, "GKDeviceConfiguration.xml", out result);
				if (!result)
				{
					AddConfigurationToZip(zipFile, gkDeviceConfiguration, "GKDeviceConfiguration.xml");
				}
				var skdConfiguration = GetConfigurationFromZip<SKDConfiguration>(zipFile, "SKDConfiguration.xml", out result);
				if (!result)
				{
					AddConfigurationToZip(zipFile, skdConfiguration, "SKDConfiguration.xml");
				}
				var skdLibraryConfiguration = GetConfigurationFromZip<SKDLibraryConfiguration>(zipFile, "SKDLibraryConfiguration.xml", out result);
				if (!result)
				{
					AddConfigurationToZip(zipFile, skdLibraryConfiguration, "SKDLibraryConfiguration.xml");
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
					var gkDeviceLibraryConfiguration = GetConfigurationFromZip<GKDeviceLibraryConfiguration>(zipFile, "GKDeviceLibraryConfiguration.xml", out result);
					if (!result)
					{
						AddConfigurationToZip(zipFile, gkDeviceLibraryConfiguration, "GKDeviceLibraryConfiguration.xml");
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
					zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("GKDeviceConfiguration", 1, 1));
					zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("GKDeviceLibraryConfiguration", 1, 1));
					zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("SKDConfiguration", 1, 1));
					zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("SKDLibraryConfiguration", 1, 1));
					zipConfigurationItemsCollection.ZipConfigurationItems.Add(new FiresecAPI.Models.ZipConfigurationItem("LayoutsConfiguration", 1, 1));
					AddConfigurationToZip(zipFile, zipConfigurationItemsCollection, "ZipConfigurationItemsCollection.xml");
				}

				if (zipFile.Entries.Any(x => x.FileName == "Info.xml"))
					zipFile.RemoveEntry("Info.xml");

				zipFile.Save(fileName);
			}
			catch (Exception e)
			{
				Logger.Error(e, "ZipConfigActualizeHelper.Actualize");
			}
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

					T configuration = ZipSerializeHelper.DeSerialize<T>(configurationMemoryStream, true);
					if (!configuration.ValidateVersion())
					{
						result = false;
					}
					return configuration;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ZipConfigActualizeHelper.GetConfigurationFomZip " + fileName);
			}
			T newConfiguration = new T();
			result = false;
			return newConfiguration;
		}

		static void AddConfigurationToZip(ZipFile zipFile, VersionedConfiguration versionedConfiguration, string fileName)
		{
			var configuarationMemoryStream = ZipSerializeHelper.Serialize(versionedConfiguration, true);
			if (zipFile.Entries.Any(x => x.FileName == fileName))
				zipFile.RemoveEntry(fileName);
			configuarationMemoryStream.Position = 0;
			zipFile.AddEntry(fileName, configuarationMemoryStream);
		}
	}
}