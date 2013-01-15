using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using Infrastructure.Common;

namespace FiresecClient
{
    public static class FileHelper
    {
		static List<string> _directoriesList = new List<string>() { "Sounds" };

        static string AppDataDirectory(string directoryName)
        {
			return Path.Combine(AppDataFolderHelper.GetClientConfigurationDirectory(), "Configuration", directoryName);
        }

        static void SynchronizeDirectory(string directoryNAme)
        {
            var remoteFileNamesList = FiresecManager.FiresecService.GetFileNamesList(directoryNAme);
            var filesDirectory = Directory.CreateDirectory(AppDataDirectory(directoryNAme));
			var filesDirectoryFullName = filesDirectory.FullName;

			foreach (var localFileName in GetFileNamesList(filesDirectoryFullName).Where(x => remoteFileNamesList.Contains(x) == false))
				File.Delete(Path.Combine(filesDirectoryFullName, localFileName));

			var localDirectoryHash = HashHelper.GetDirectoryHash(filesDirectory.FullName);
            foreach (var remoteFileHash in FiresecManager.FiresecService.GetDirectoryHash(directoryNAme).Where(x => localDirectoryHash.ContainsKey(x.Key) == false))
            {
				var fileName = Path.Combine(filesDirectoryFullName, remoteFileHash.Value);
                if (File.Exists(fileName))
                    File.Delete(fileName);
				DownloadFile(Path.Combine(directoryNAme, remoteFileHash.Value), fileName);
            }
        }

        static void DownloadFile(string sourcePath, string destinationPath)
        {
            using (var stream = FiresecManager.FiresecService.GetFile(sourcePath))
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