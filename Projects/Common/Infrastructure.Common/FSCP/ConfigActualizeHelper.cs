using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ionic.Zip;
using System.IO;
using FiresecAPI;
using FiresecAPI.Models;
using Common;
using XFiresecAPI;

namespace Infrastructure.Common
{
	public static class ConfigActualizeHelper
	{
		public static void Actualize(string fileName)
		{
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var securityConfiguration = GetFile<SecurityConfiguration>(zipFile, "SecurityConfiguration.xml");
			if (securityConfiguration == null)
			{
				securityConfiguration = new SecurityConfiguration();
				Add(zipFile, securityConfiguration, "SecurityConfiguration.xml");
			}

			var plansConfiguration = GetFile<PlansConfiguration>(zipFile, "PlansConfiguration.xml");
			if (plansConfiguration == null)
			{
				plansConfiguration = new PlansConfiguration();
				Add(zipFile, plansConfiguration, "PlansConfiguration.xml");
			}

			var systemConfiguration = GetFile<SystemConfiguration>(zipFile, "SystemConfiguration.xml");
			if (systemConfiguration == null)
			{
				systemConfiguration = new SystemConfiguration();
				Add(zipFile, systemConfiguration, "SystemConfiguration.xml");
			}

			var driversConfiguration = GetFile<DriversConfiguration>(zipFile, "DriversConfiguration.xml");
			if (driversConfiguration == null)
			{
				driversConfiguration = new DriversConfiguration();
				Add(zipFile, driversConfiguration, "DriversConfiguration.xml");
			}

			var deviceConfiguration = GetFile<DeviceConfiguration>(zipFile, "DeviceConfiguration.xml");
			if (deviceConfiguration == null)
			{
				deviceConfiguration = new DeviceConfiguration();
				Add(zipFile, deviceConfiguration, "DeviceConfiguration.xml");
			}

			var deviceLibraryConfiguration = GetFile<DeviceLibraryConfiguration>(zipFile, "DeviceLibraryConfiguration.xml");
			if (deviceLibraryConfiguration == null)
			{
				deviceLibraryConfiguration = new DeviceLibraryConfiguration();
				Add(zipFile, deviceLibraryConfiguration, "DeviceLibraryConfiguration.xml");
			}

			var xDeviceConfiguration = GetFile<XDeviceConfiguration>(zipFile, "XDeviceConfiguration.xml");
			if (xDeviceConfiguration == null)
			{
				xDeviceConfiguration = new XDeviceConfiguration();
				Add(zipFile, xDeviceConfiguration, "XDeviceConfiguration.xml");
			}

			var xDeviceLibraryConfiguration = GetFile<XDeviceLibraryConfiguration>(zipFile, "XDeviceLibraryConfiguration.xml");
			if (xDeviceLibraryConfiguration == null)
			{
				xDeviceLibraryConfiguration = new XDeviceLibraryConfiguration();
				Add(zipFile, xDeviceLibraryConfiguration, "XDeviceLibraryConfiguration.xml");
			}

            zipFile.Save(fileName);
		}

		static T GetFile<T>(ZipFile zipFile, string fileName)
		where T : VersionedConfiguration, new()
		{
			try
			{
				var configurationEntry = zipFile[fileName];
				if (configurationEntry != null)
				{
					var configurationMemoryStream = new MemoryStream();
					configurationEntry.Extract(configurationMemoryStream);
					configurationMemoryStream.Position = 0;
					return SerializeHelper.DeSerialize<T>(configurationMemoryStream);
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "ConfigActualizeHelper.GetFile " + fileName);
			}
			return null;
		}

		static void Add(ZipFile zipFile, VersionedConfiguration versionedConfiguration, string fileName)
		{
			var configuarationMemoryStream = SerializeHelper.Serialize(versionedConfiguration);
			if (zipFile.Entries.FirstOrDefault(x => x.FileName == fileName) != null)
				zipFile.RemoveEntry(fileName);
			configuarationMemoryStream.Position = 0;
			zipFile.AddEntry(fileName, configuarationMemoryStream);
		}
	}
}