using System.IO;
using System.Text;
using FiresecAPI.Models;
using Infrastructure.Common;
using Ionic.Zip;
using XFiresecAPI;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void LoadFromZipFile(string fileName)
		{
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });

			var zipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
			var infoMemoryStream = new MemoryStream();
			var entry = zipFile["ZipConfigurationItemsCollection.xml"];
			if (entry != null)
			{
				entry.Extract(infoMemoryStream);
				infoMemoryStream.Position = 0;
				zipConfigurationItemsCollection = ZipSerializeHelper.DeSerialize<ZipConfigurationItemsCollection>(infoMemoryStream);
			}

			foreach (var zipConfigurationItem in zipConfigurationItemsCollection.GetWellKnownZipConfigurationItems)
			{
				var configurationEntry = zipFile[zipConfigurationItem.Name];
				if (configurationEntry != null)
				{
					var configurationMemoryStream = new MemoryStream();
					configurationEntry.Extract(configurationMemoryStream);
					configurationMemoryStream.Position = 0;
					switch (zipConfigurationItem.Name)
					{
						case "SecurityConfiguration.xml":
							SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(configurationMemoryStream);
							break;

						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationMemoryStream);
							break;

						case "SystemConfiguration.xml":
							SystemConfiguration = ZipSerializeHelper.DeSerialize<SystemConfiguration>(configurationMemoryStream);
							break;

						case "DriversConfiguration.xml":
							FiresecConfiguration.DriversConfiguration = ZipSerializeHelper.DeSerialize<DriversConfiguration>(configurationMemoryStream);
							break;

						case "DeviceConfiguration.xml":
							FiresecConfiguration.DeviceConfiguration = ZipSerializeHelper.DeSerialize<DeviceConfiguration>(configurationMemoryStream);
							break;

						case "DeviceLibraryConfiguration.xml":
							DeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<DeviceLibraryConfiguration>(configurationMemoryStream);
							break;

						case "XDeviceConfiguration.xml":
							XManager.DeviceConfiguration = ZipSerializeHelper.DeSerialize<XDeviceConfiguration>(configurationMemoryStream);
							break;

						case "XDeviceLibraryConfiguration.xml":
							XManager.XDeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<XDeviceLibraryConfiguration>(configurationMemoryStream);
							break;
					}
				}
			}

			zipFile.Dispose();
		}
	}
}