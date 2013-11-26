using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Ionic.Zip;
using XFiresecAPI;


namespace Infrastructure.Common
{
	public static class ZipFileConfigurationHelper
	{
		public static void SaveToZipFile(string fileName, XDeviceConfiguration deviceConfiguration)
		{
			var folderName = AppDataFolderHelper.GetTempFolder();
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

			if (File.Exists(fileName))
				File.Delete(fileName);

			TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
			AddConfiguration(folderName, "XDeviceConfiguration.xml", deviceConfiguration, 1, 1);
			var zipFile = new ZipFile(fileName);
			zipFile.AddDirectory(folderName);
			zipFile.Save(fileName);
			zipFile.Dispose();
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
		}

		static ZipConfigurationItemsCollection TempZipConfigurationItemsCollection = new ZipConfigurationItemsCollection();
		static void AddConfiguration(string folderName, string name, VersionedConfiguration configuration, int minorVersion, int majorVersion)
		{
			configuration.BeforeSave();
			configuration.Version = new ConfigurationVersion() { MinorVersion = minorVersion, MajorVersion = majorVersion };
			ZipSerializeHelper.Serialize(configuration, Path.Combine(folderName, name));
			TempZipConfigurationItemsCollection.ZipConfigurationItems.Add(new ZipConfigurationItem(name, minorVersion, majorVersion));
		}

		public static XDeviceConfiguration LoadFromZipFile(string fileName)
		{
			var deviceConfiguration = new XDeviceConfiguration();
			var zipFile = ZipFile.Read(fileName, new ReadOptions { Encoding = Encoding.GetEncoding("cp866") });
			var fileInfo = new FileInfo(fileName);
			var unzipFolderPath = Path.Combine(fileInfo.Directory.FullName, "Unzip\\FileFromGK");
			zipFile.ExtractAll(unzipFolderPath);
			var fileXml = new DirectoryInfo(unzipFolderPath).GetFiles().FirstOrDefault();
			if (fileXml != null)
			{
				var configurationFileName = Path.Combine(unzipFolderPath, fileXml.Name);
				deviceConfiguration = ZipSerializeHelper.DeSerialize<XDeviceConfiguration>(configurationFileName);
			}

			foreach (var file in new DirectoryInfo(unzipFolderPath).GetFiles())
				file.Delete();
			Directory.Delete(unzipFolderPath);

			zipFile.Dispose();
			return deviceConfiguration;
		}
	}
}
