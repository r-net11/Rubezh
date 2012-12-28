using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;
using Common;
using Ionic.Zip;
using FiresecClient;
using FiresecAPI;

namespace FireAdministrator
{
	public static class ConfigFileHelper
	{
		public static void SaveToFile()
		{
			try
			{
				var deviceConfigurationStream = SerializeHelper.Serialize(FiresecManager.FiresecConfiguration.DeviceConfiguration);
				var plansConfigurationStream = SerializeHelper.Serialize(FiresecManager.PlansConfiguration);
				var systemConfigurationStream = SerializeHelper.Serialize(FiresecManager.SystemConfiguration);
				var xDeviceConfigurationStream = SerializeHelper.Serialize(XManager.DeviceConfiguration);

				var zip = new ZipFile("TempConfig.fscp");

				Add(zip, FiresecManager.SystemConfiguration, "SystemConfiguration.xml");
				Add(zip, FiresecManager.SecurityConfiguration, "SecurityConfiguration.xml");
				Add(zip, FiresecManager.PlansConfiguration, "PlansConfiguration.xml");
				Add(zip, FiresecManager.FiresecConfiguration.DeviceConfiguration, "DeviceConfiguration.xml");
				Add(zip, FiresecManager.DeviceLibraryConfiguration, "DeviceLibraryConfiguration.xml");
				Add(zip, XManager.DeviceConfiguration, "XDeviceConfiguration.xml");
				Add(zip, XManager.XDeviceLibraryConfiguration, "XDeviceLibraryConfiguration.xml");

				var configList = new ConfigurationsList();
				configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "SystemConfiguration" });
				configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "SecurityConfiguration" });
				configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "PlansConfiguration" });
				configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "DeviceConfiguration" });
				configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "DeviceLibraryConfiguration" });
				configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "XDeviceConfiguration" });
				configList.Configurations.Add(new FiresecAPI.Models.Configuration() { MajorVersion = 1, MinorVersion = 1, Name = "XDeviceLibraryConfiguration" });
				var ms = SerializeHelper.Serialize(configList);
				ms.Position = 0;
				if (zip.Entries.FirstOrDefault(x => x.FileName == "Info.xml") != null)
					zip.RemoveEntry("Info.xml");
				zip.AddEntry("Info.xml", ms);
				zip.Save("TempConfig.fscp");
			}
			catch (Exception e)
			{
				Logger.Error(e, "MenuView.SaveToFile");
			}
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