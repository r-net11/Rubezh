using System.Collections.Generic;
using System.IO;
using Common;

namespace FiresecClient
{
    public class FileHelper
    {
        public FileHelper()
        {
            _directoriesList = new List<string>() { "Sounds" };
        }

        List<string> _directoriesList;

        string CurrentDirectory(string directory)
        {
            return Directory.GetCurrentDirectory() + @"\" + directory;
        }

        void SynchronizeDirectory(string directory)
        {
            var filesDirectory = Directory.CreateDirectory(CurrentDirectory(directory));
            var localDirectoryHash = HashHelper.GetDirectoryHash(directory);
            var remoteDirectoryHash = FiresecManager.GetDirectoryHash(directory);

            foreach (var remoteFileHash in remoteDirectoryHash)
            {
                if (localDirectoryHash.ContainsKey(remoteFileHash.Key) == false)
                {
                    DownloadFile(filesDirectory.Name + @"\" + remoteFileHash.Value, filesDirectory.FullName + @"\" + remoteFileHash.Value);
                }
            }

            foreach (var localFileHash in localDirectoryHash)
            {
                if (remoteDirectoryHash.ContainsKey(localFileHash.Key) == false)
                {
                    File.Delete(filesDirectory.FullName + localFileHash.Value);
                }
            }
        }

        //Почему такое название? Ведь метод копирует содержимое одного файла в другой. Очевидное название CopyFile
        //И если что-нибудь поменяется и по пути directoryAndFileName не будет файла, то по ходу все упадет
        void DownloadFile(string directoryAndFileName, string destinationPath)
        {
            var stream = FiresecManager.GetFile(directoryAndFileName);
            using (var destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(destinationStream);
            }
        }

        List<string> GetFileNamesList(string directory)
        {
            var fileNames = new List<string>();
            foreach (var str in Directory.EnumerateFiles(CurrentDirectory(directory)))
            {
                fileNames.Add(Path.GetFileName(str));
            }

            return fileNames;
        }

        public void Synchronize()
        {
            foreach (var directory in _directoriesList)
            {
                SynchronizeDirectory(directory);
            }
        }

        public List<string> SoundsList
        {
            get { return GetFileNamesList(_directoriesList[0]); }
        }

        public string GetSoundFilePath(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                return null;
            }
            else
            {
                return CurrentDirectory(_directoriesList[0]) + @"\" + file;
            }
        }
    }
}