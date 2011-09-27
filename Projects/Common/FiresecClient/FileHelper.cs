using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var remoteFileNamesList = FiresecManager.GetFileNamesList(directory);
            var filesDirectory = Directory.CreateDirectory(CurrentDirectory(directory));

            foreach (var localFileName in GetFileNamesList(directory).Where(x => remoteFileNamesList.Contains(x) == false))
                File.Delete(Path.Combine(filesDirectory.FullName, localFileName));

            var localDirectoryHash = HashHelper.GetDirectoryHash(directory);
            foreach (var remoteFileHash in FiresecManager.GetDirectoryHash(directory).Where(x => localDirectoryHash.ContainsKey(x.Key) == false))
            {
                var fileName = Path.Combine(filesDirectory.FullName, remoteFileHash.Value);
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }
                DownloadFile(Path.Combine(filesDirectory.Name, remoteFileHash.Value), fileName);
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
            if (Directory.Exists(CurrentDirectory(directory)))
            {
                return new List<string>(
                    Directory.EnumerateFiles(CurrentDirectory(directory)).Select(x => Path.GetFileName(x))
                );
            }
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

        public static string GetIconFilePath(string fileName)
        {
            return string.IsNullOrWhiteSpace(fileName) ? null : Path.Combine(CurrentDirectory(_directoriesList[1]), fileName);
        }

        public static string GetSoundFilePath(string fileName)
        {
            return string.IsNullOrWhiteSpace(fileName) ? null : Path.Combine(CurrentDirectory(_directoriesList[0]), fileName);
        }
    }
}