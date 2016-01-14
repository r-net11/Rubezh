using System.IO;
using System.Text;
using RubezhAPI.GK;
using RubezhAPI.Models;
using Infrastructure.Common;
using Ionic.Zip;
using RubezhAPI;

namespace RubezhClient
{
	public partial class ClientManager
	{
		public static void LoadFromZipFile(string fileName)
		{
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(fileName);
			var unzipFolderPath = fileInfo.Directory.FullName;
			zipFile.ExtractAll(unzipFolderPath);
			zipFile.Dispose();
			LoadConfigFromDirectory(unzipFolderPath);
		}

		static void LoadConfigFromDirectory(string unzipFolderPath)
		{
        	foreach (var zipConfigurationItem in ZipConfigurationItemsCollection.GetWellKnownNames())
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem)
					{
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName);
							break;
						case "SystemConfiguration.xml":
							SystemConfiguration = ZipSerializeHelper.DeSerialize<SystemConfiguration>(configurationFileName);
							break;
						case "GKDeviceConfiguration.xml":
							GKManager.DeviceConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(configurationFileName);
							break;
						case "GKDeviceLibraryConfiguration.xml":
							GKManager.DeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceLibraryConfiguration>(configurationFileName);
							break;
						case "LayoutsConfiguration.xml":
							LayoutsConfiguration = ZipSerializeHelper.DeSerialize<LayoutsConfiguration>(configurationFileName);
							break;
					}
				}
			}
		}
	}
}