using System;
using System.Collections.Generic;
using System.IO;
using Common;

namespace FiresecClient
{
    public static class FileHelper
    {
        static FileHelper()
        {
            _directoriesList = new List<string>() { "Sounds", "Icons" };
        }

        static List<string> _directoriesList;

        static string CurrentDirectory(string directory)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configuration", directory);
        }

        static void SynchronizeDirectory(string directory)
        {
            var filesDirectory = Directory.CreateDirectory(CurrentDirectory(directory));

            var remoteFileNamesList = FiresecManager.GetFileNamesList(directory);
            var localFileNamesList = GetFileNamesList(directory);
            foreach (var localFileName in localFileNamesList)
            {
                if (remoteFileNamesList.Contains(localFileName) == false)
                {
                    File.Delete(Path.Combine(filesDirectory.FullName, localFileName));
                }
            }
            var localDirectoryHash = HashHelper.GetDirectoryHash(directory);
            var remoteDirectoryHash = FiresecManager.GetDirectoryHash(directory);

            if (remoteDirectoryHash.IsNotNullOrEmpty())
            {
                foreach (var remoteFileHash in remoteDirectoryHash)
                {
                    if (localDirectoryHash.ContainsKey(remoteFileHash.Key) == false)
                    {
                        var fileName = Path.Combine(filesDirectory.FullName, remoteFileHash.Value);
                        if (File.Exists(fileName))
                        {
                            File.Delete(fileName);
                        }
                        DownloadFile(filesDirectory.Name + @"\" + remoteFileHash.Value, fileName);
                    }
                }
            }
        }

        static void DownloadFile(string sourcePath, string destinationPath)
        {
            using (var stream = FiresecManager.GetFile(sourcePath))
            using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(destinationStream);
            }
        }

        static List<string> GetFileNamesList(string directory)
        {
            var fileNames = new List<string>();
            if (Directory.Exists(CurrentDirectory(directory)))
            {
                foreach (var str in Directory.EnumerateFiles(CurrentDirectory(directory)))
                {
                    fileNames.Add(Path.GetFileName(str));
                }
            }
            return fileNames;
        }

        public static void Synchronize()
        {
            foreach (var directory in _directoriesList)
            {
                SynchronizeDirectory(directory);
            }
        }

        public static List<string> SoundsList
        {
            get { return GetFileNamesList(_directoriesList[0]); }
        }

        public static string GetIconFilePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }
            return CurrentDirectory(_directoriesList[1]) + @"\" + fileName;
        }

        public static string GetSoundFilePath(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }
            return CurrentDirectory(_directoriesList[0]) + @"\" + fileName;
        }
    }
}