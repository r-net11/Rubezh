using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using FiresecClient;
using System.Security.Cryptography;

namespace SoundsModule.ViewModels
{
    public static class DownloadHelper
    {
        public static string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory() + @"\Sounds\"; }
        }

        public const string DefaultName = "<не задано>";

        public const bool DefaultIsContinious = false;

        public static ObservableCollection<string> GetAvailableStates
        {
            get
            {
                ObservableCollection<string> availableStates = new ObservableCollection<string>();
                availableStates.Add("Тревога");
                availableStates.Add("Внимание");
                availableStates.Add("Неисправность");
                availableStates.Add("Требуется обслуживание");
                availableStates.Add("Отключено");
                availableStates.Add("Неизвестно");
                availableStates.Add("Норма(*)");
                availableStates.Add("Норма");
                return availableStates;
            }

        }

        public static ObservableCollection<string> GetAvailableSounds
        {
            get
            {
                ObservableCollection<string> fileNames = new ObservableCollection<string>();
                fileNames.Add(DefaultName);
                foreach (string str in Directory.GetFiles(CurrentDirectory))
                {
                    fileNames.Add(Path.GetFileName(str));
                }
                return fileNames;
            }
        }

        public static ObservableCollection<string> AvailableSpeakers
        {
            get
            {
                ObservableCollection<string> availableSpeakers = new ObservableCollection<string>();
                availableSpeakers.Add(DownloadHelper.DefaultName);
                availableSpeakers.Add("Тревога");
                availableSpeakers.Add("Внимание");
                return availableSpeakers;
            }
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

        private static void LoadFile(string file, string destinationPath)
        {
            Stream stream = FiresecManager.GetFile(file);
            FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(destinationStream);
            destinationStream.Close();
        }

        private static Dictionary<string, string> GetFileHash()
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
