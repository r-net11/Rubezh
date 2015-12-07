using System.IO;
using System.Text;
using Common;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using Infrastructure.Common;
using Ionic.Zip;
using Infrastructure.Common.Windows;
using RubezhAPI;
using System.Xml.Linq;

namespace RubezhClient
{
	public partial class ClientManager
	{
		public static void LoadFromZipFile(string fileName, string path = null, XDocument xmlDoc = null )
		{
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(fileName);
			var unzipFolderPath = fileInfo.Directory.FullName;
			zipFile.ExtractAll(unzipFolderPath);
			if (xmlDoc != null && path!= null)
				xmlDoc.Save(path);
			zipFile.Dispose();
			LoadConfigFromDirectory(unzipFolderPath);
		}

		static void LoadConfigFromDirectory(string unzipFolderPath)
		{
        	foreach (var zipConfigurationItem in ZipConfigurationIteАДМ. Ошибки и недочеты прав доступаmsCollection.GetWellKnownNames())
			{
				var configurationFileName = Path.Combine(unzipFolderPath, zipConfigurationItem);
				if (File.Exists(configurationFileName))
				{
					switch (zipConfigurationItem)
					{
						case "SecurityConfiguration.xml":
							SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(configurationFileName, true);
							break;
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName, true);
							break;
						case "SystemConfiguration.xml":
							SystemConfiguration = ZipSerializeHelper.DeSerialize<SystemConfiguration>(configurationFileName, true);
							break;
						case "GKDeviceConfiguration.xml":
							GKManager.DeviceConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceConfiguration>(configurationFileName, true);
							break;
						case "GKDeviceLibraryConfiguration.xml":
							GKManager.DeviceLibraryConfiguration = ZipSerializeHelper.DeSerialize<GKDeviceLibraryConfiguration>(configurationFileName, true);
							break;
						case "LayoutsConfiguration.xml":
							LayoutsConfiguration = ZipSerializeHelper.DeSerialize<LayoutsConfiguration>(configurationFileName, false);
							break;
					}
				}
			}
		}
	}
}