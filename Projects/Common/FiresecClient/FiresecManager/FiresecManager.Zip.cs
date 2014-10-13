using System.IO;
using System.Text;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Ionic.Zip;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static void LoadFromZipFile(string fileName, out bool isFullConfiguration)
		{
			isFullConfiguration = false;
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(fileName);
			var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip");
			zipFile.ExtractAll(unzipFolderPath);

			var zipConfigurationItemsCollectionFileName = Path.Combine(unzipFolderPath, "ZipConfigurationItemsCollection.xml");
			if (!File.Exists(zipConfigurationItemsCollectionFileName))
			{
				Logger.Error("FiresecManager.LoadFromZipFile zipConfigurationItemsCollectionFileName file not found");
				return;
			}
			var zipConfigurationItemsCollection = ZipSerializeHelper.DeSerialize<ZipConfigurationItemsCollection>(zipConfigurationItemsCollectionFileName, true);
			if (zipConfigurationItemsCollection == null)
			{
				Logger.Error("FiresecManager.LoadFromZipFile zipConfigurationItemsCollection == null");
				return;
			}

			foreach (var zipConfigurationItem in zipConfigurationItemsCollection.GetWellKnownZipConfigurationItems())
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem.Name);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem.Name)
					{
						case "SecurityConfiguration.xml":
							SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(configurationFileName, true);
							isFullConfiguration = true;
							break;
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName, true);
							break;
						case "SystemConfiguration.xml":
							SystemConfiguration = ZipSerializeHelper.DeSerialize<SystemConfiguration>(configurationFileName, true);
							break;
						case "DriversConfiguration.xml":
							FiresecConfiguration.DriversConfiguration = ZipSerializeHelper.DeSerialize<DriversConfiguration>(configurationFileName, true);
							break;
						case "DeviceConfiguration.xml":
							FiresecConfiguration.DeviceConfiguration = ZipSerializeHelper.DeSerialize<DeviceConfiguration>(configurationFileName, true);
							break;
						case "DeviceLibraryConfiguration.xml":
							DeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<DeviceLibraryConfiguration>(configurationFileName, true);
							break;
						case "GKDeviceConfiguration.xml":
							GKManager.DeviceConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(configurationFileName, true);
							break;
						case "GKDeviceLibraryConfiguration.xml":
							GKManager.DeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceLibraryConfiguration>(configurationFileName, true);
							break;
						case "SKDConfiguration.xml":
							SKDManager.SKDConfiguration = ZipSerializeHelper.DeSerialize<SKDConfiguration>(configurationFileName, true);
							break;
						case "SKDLibraryConfiguration.xml":
							SKDManager.SKDLibraryConfiguration = ZipSerializeHelper.DeSerialize<SKDLibraryConfiguration>(configurationFileName, true);
							break;
						case "LayoutsConfiguration.xml":
							LayoutsConfiguration = ZipSerializeHelper.DeSerialize<LayoutsConfiguration>(configurationFileName, false);
							break;
					}
				}
			}

			zipFile.Dispose();
		}
	}
}