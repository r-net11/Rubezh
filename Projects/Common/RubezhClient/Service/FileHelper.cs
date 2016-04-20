using Common;
using Infrastructure.Common.Windows;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RubezhClient
{
	public static class FileHelper
	{
		static List<string> _directoriesList = new List<string>() { "Sounds" };

		static string AppDataDirectory(string directoryName)
		{
			return Path.Combine(AppDataFolderHelper.GetClientConfigurationDirectory(), directoryName);
		}

		static void SynchronizeDirectory(string directoryName)
		{
			var directoryInfo = Directory.CreateDirectory(AppDataDirectory(directoryName));
			var fullDirectoryName = directoryInfo.FullName;
			var remoteFileNamesList = ClientManager.FiresecService.GetFileNamesList(directoryName);

			foreach (var localFileName in GetFileNamesList(fullDirectoryName).Where(x => remoteFileNamesList.Contains(x) == false))
				File.Delete(Path.Combine(fullDirectoryName, localFileName));

			var localDirectoryHash = HashHelper.GetDirectoryHash(directoryInfo.FullName);
			foreach (var remoteFileHash in ClientManager.FiresecService.GetDirectoryHash(directoryName).Where(x => localDirectoryHash.ContainsKey(x.Key) == false))
			{
				var fileName = Path.Combine(fullDirectoryName, remoteFileHash.Value);
				if (File.Exists(fileName))
					File.Delete(fileName);
				DownloadFile(Path.Combine(directoryName, remoteFileHash.Value), fileName);
			}
		}

		static void DownloadFile(string sourcePath, string destinationPath)
		{
			using (var stream = ClientManager.FiresecService.GetServerAppDataFile(sourcePath))
			using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
			{
				stream.CopyTo(destinationStream);
			}
		}

		static List<string> GetFileNamesList(string directory)
		{
			if (Directory.Exists(AppDataDirectory(directory)))
				return new List<string>(Directory.EnumerateFiles(AppDataDirectory(directory)).Select(x => Path.GetFileName(x)));
			return new List<string>();
		}

		public static void Synchronize()
		{
			_directoriesList.ForEach(x => SynchronizeDirectory(x));
		}

		public static List<string> SoundsList
		{
			get { return GetFileNamesList(_directoriesList[0]); }
		}

		public static string GetSoundFilePath(string fileName)
		{
			return string.IsNullOrWhiteSpace(fileName) ? null : Path.Combine(AppDataDirectory(_directoriesList[0]), fileName);
		}
	}
}