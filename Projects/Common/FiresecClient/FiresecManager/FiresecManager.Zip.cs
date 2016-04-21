using FiresecAPI.Models;
using FiresecAPI.SKD;
using Infrastructure.Common;
using Ionic.Zip;
using System.IO;
using System.Text;

namespace FiresecClient
{
	public partial class FiresecManager
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
						case "SecurityConfiguration.xml":
							SecurityConfiguration = ZipSerializeHelper.DeSerialize<SecurityConfiguration>(configurationFileName, true);
							break;
						case "PlansConfiguration.xml":
							PlansConfiguration = ZipSerializeHelper.DeSerialize<PlansConfiguration>(configurationFileName, true);
							break;
						case "SystemConfiguration.xml":
							SystemConfiguration = ZipSerializeHelper.DeSerialize<SystemConfiguration>(configurationFileName, false);
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
		}
	}
}