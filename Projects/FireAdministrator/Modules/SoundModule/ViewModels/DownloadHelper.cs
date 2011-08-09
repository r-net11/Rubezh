using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using FiresecClient;

namespace SoundsModule.ViewModels
{
    public static class DownloadHelper
    {
        public const string DefaultName = "<не задано>";

        public const bool DefaultIsContinious = false;

        public static string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory() + @"\Sounds\"; }
        }

        public static void UpdateSound()
        {
            DirectoryInfo soundDirectory = Directory.CreateDirectory(CurrentDirectory);
            Dictionary<string, string> HashTableSoundFiles = new Dictionary<string, string>();
            HashTableSoundFiles = GetFileHash();
            Dictionary<string, string> HashTableSoundFilesFromServer = new Dictionary<string, string>();
            HashTableSoundFilesFromServer = FiresecManager.GetHashAndNameSoundFiles();

            foreach (KeyValuePair<string, string> kvp in HashTableSoundFilesFromServer)
            {
                if (!HashTableSoundFiles.ContainsKey(kvp.Key))
                {
                    LoadFile(kvp.Value, soundDirectory.FullName + kvp.Value);
                }
            }

            foreach (KeyValuePair<string, string> kvp in HashTableSoundFiles)
            {
                if (!HashTableSoundFilesFromServer.ContainsKey(kvp.Key))
                {
                    File.Delete(soundDirectory.FullName + kvp.Value);
                }
            }
        }

        static void LoadFile(string file, string destinationPath)
        {
            Stream stream = FiresecManager.GetFile(file);
            FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(destinationStream);
            destinationStream.Close();
        }

        static Dictionary<string, string> GetFileHash()
        {
            Dictionary<string, string> hashTable = new Dictionary<string, string>();
            List<string> HashListSoundFiles = new List<string>();
            DirectoryInfo dir = new DirectoryInfo(CurrentDirectory);
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
    }
}