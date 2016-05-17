using System;
using System.IO;

namespace Infrastructure.Common
{
	public class AppDataFolderHelper
	{
		static string AppDataFolderName;

		static AppDataFolderHelper()
		{
			var appDataFolderName = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
			AppDataFolderName = Path.Combine(appDataFolderName, "Rubezh");
		}

		public static string GetLocalFolder(string folderName)
		{
			return GetFolder(Environment.UserName + "_" + folderName);
		}

		public static string GetFolder(string folderName)
		{
			return Path.Combine(AppDataFolderName, folderName);
		}

		public static string GetFile(string fileName)
		{
			return Path.Combine(AppDataFolderName, fileName);
		}

		public static string GetTempFileName()
		{
			return Path.Combine(AppDataFolderName, "Temp", Path.GetTempFileName());
		}

		public static string GetTempFolder()
		{
			return Path.Combine(AppDataFolderName, "Temp", Guid.NewGuid().ToString());
		}

		public static string GetClientConfigurationDirectory()
		{
			return Path.Combine(AppDataFolderName, "CommonClientConfiguration");
		}

		public static string GetMonitorSettingsPath(string fileName = null)
		{
			var filePath = Path.Combine(AppDataFolderName, "Monitor", "Settings");
			if (!string.IsNullOrEmpty(fileName))
				filePath = Path.Combine(filePath, fileName);
			return filePath;
		}

		public static string GetServerAppDataPath(string fileOrDirectoryName = null)
		{
			var fileName = Path.Combine(AppDataFolderName, "Server");
			if (!string.IsNullOrEmpty(fileOrDirectoryName))
				fileName = Path.Combine(fileName, fileOrDirectoryName);
			return fileName;
		}

		public static string GetLogsFolder(string folderName = null)
		{
			if (folderName == null)
				return Path.Combine(AppDataFolderName, "Logs");
			return Path.Combine(AppDataFolderName, "Logs", folderName);
		}

		public static string GetRegistryDataConfigurationFileName()
		{
			return Path.Combine(AppDataFolderName, "RegistryDataConfiguration.xml");
		}

		public static string GetGlobalSettingsFileName()
		{
			return Path.Combine(AppDataFolderName, "GlobalSettings.xml");
		}

		public static string GetFileInFolder(string folderName, string fileName)
		{
			return Path.Combine(AppDataFolderName, folderName, fileName);
		}
	}
}