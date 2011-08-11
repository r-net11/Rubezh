using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

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

        void UpdateFiles(string directory)
        {
            DirectoryInfo filesDirectory = Directory.CreateDirectory(CurrentDirectory(directory));
            var HashTableFiles = new Dictionary<string, string>();
            HashTableFiles = GetFileHash(directory);
            var HashTableFilesFromServer = new Dictionary<string, string>();
            HashTableFilesFromServer = FiresecManager.GetHashAndNameFiles(directory);

            foreach (var kvp in HashTableFilesFromServer)
            {
                if (!HashTableFiles.ContainsKey(kvp.Key))
                {
                    LoadFile(filesDirectory.Name + @"\" + kvp.Value, filesDirectory.FullName + @"\" + kvp.Value);
                }
            }

            foreach (var kvp in HashTableFiles)
            {
                if (!HashTableFilesFromServer.ContainsKey(kvp.Key))
                {
                    File.Delete(filesDirectory.FullName + kvp.Value);
                }
            }
        }

        void LoadFile(string directoryAndFileName, string destinationPath)
        {
            Stream stream = FiresecManager.GetFile(directoryAndFileName);
            FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(destinationStream);
            destinationStream.Close();
        }

        Dictionary<string, string> GetFileHash(string directory)
        {
            Dictionary<string, string> hashTable = new Dictionary<string, string>();
            List<string> HashListFiles = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(CurrentDirectory(directory));
            FileInfo[] files = dir.GetFiles();
            byte[] hash;
            StringBuilder sBuilder = new StringBuilder();
            foreach (FileInfo fInfo in files)
            {
                sBuilder.Clear();
                using (FileStream fileStream = fInfo.Open(FileMode.Open))
                {
                    hash = MD5.Create().ComputeHash(fileStream);
                    for (int i = 0; i < hash.Length; i++)
                    {
                        sBuilder.Append(hash[i].ToString());
                    }
                }
                hashTable.Add(sBuilder.ToString(), fInfo.Name);
            }
            return hashTable;
        }
        
        List<string> GetFileNamesList(string directory)
        {
            List<string> fileNames = new List<string>();
            foreach (var str in Directory.GetFiles(CurrentDirectory(directory)))
            {
                fileNames.Add(Path.GetFileName(str));
            }
            return fileNames;
        }

        public void Sinchronize()
        {
            foreach (var directory in _directoriesList)
            {
                UpdateFiles(directory);
            }
        }

        public List<string> GetListSounds
        {
            get { return GetFileNamesList(_directoriesList[0]); }
        }

        public string GetFilePath(string file)
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
