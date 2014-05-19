using System.IO;
using System.Linq;
using FiresecAPI;
using FiresecAPI.GK;
using Ionic.Zip;


namespace Infrastructure.Common
{
	public static class ZipFileConfigurationHelper
	{
		public static string Error { get; private set; }
		public static void SaveToZipFile(string fileName, XDeviceConfiguration deviceConfiguration)
		{
			var folderName = AppDataFolderHelper.GetTempFolder();
			if (!Directory.Exists(folderName))
				Directory.CreateDirectory(folderName);

			if (File.Exists(fileName))
				File.Delete(fileName);

			deviceConfiguration.BeforeSave();
			deviceConfiguration.Version = new ConfigurationVersion() { MinorVersion = 1, MajorVersion = 1 };
			ZipSerializeHelper.Serialize(deviceConfiguration, Path.Combine(folderName, "XDeviceConfiguration.xml"));
			var zipFile = new ZipFile(fileName);
			zipFile.AddDirectory(folderName);
			zipFile.Save(fileName);
			zipFile.Dispose();
			if (Directory.Exists(folderName))
				Directory.Delete(folderName, true);
		}

		public static XDeviceConfiguration UnZipFromStream(MemoryStream memoryStream)
		{
			var zipFile = ZipFile.Read(memoryStream);
			var dataMemory = new MemoryStream();
			var firstOrDefault = zipFile.FirstOrDefault();
			if (firstOrDefault != null) firstOrDefault.Extract(dataMemory);
			dataMemory.Position = 0;
			try
			{
				return ZipSerializeHelper.DeSerialize<XDeviceConfiguration>(dataMemory);
			}
			catch
			{
				Error = "Не удалось десериализовать xml конфигурацию";
				return null;
			}
			finally
			{
				memoryStream.Close();
				zipFile.Dispose();
				dataMemory.Close();
			}
		}
	}
}