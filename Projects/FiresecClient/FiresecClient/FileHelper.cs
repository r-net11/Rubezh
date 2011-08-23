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
            return Directory.GetCurrentDirectory() + @"\Configuration\" + directory;
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
                    File.Delete(filesDirectory.FullName + @"\" + localFileName);
                }
            }

            var localDirectoryHash = HashHelper.GetDirectoryHash(directory);
            var remoteDirectoryHash = FiresecManager.GetDirectoryHash(directory);

            if (remoteDirectoryHash != null)
            {
                foreach (var remoteFileHash in remoteDirectoryHash)
                {
                    if (localDirectoryHash != null)
                    {
                        if (localDirectoryHash.ContainsKey(remoteFileHash.Key) == false)
                        {
                            if (File.Exists(filesDirectory.FullName + @"\" + remoteFileHash.Value))
                            {
                                File.Delete(filesDirectory.FullName + @"\" + remoteFileHash.Value);
                            }
                            DownloadFile(filesDirectory.Name + @"\" + remoteFileHash.Value, filesDirectory.FullName + @"\" + remoteFileHash.Value);
                        }
                    }
                }
            }
        }

        static void DownloadFile(string directoryAndFileName, string destinationPath)
        {
            var stream = FiresecManager.GetFile(directoryAndFileName);
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