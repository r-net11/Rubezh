using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using FiresecClient;
using System.Security.Cryptography;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    public class SoundsLoadViewModel
    {
        public SoundsLoadViewModel()
        {
            LoadSoundCommand = new RelayCommand(OnLoadSound);
        }

        public void Inicialized(ObservableCollection<string> availableStates,
            ObservableCollection<string> availableSounds,
            ObservableCollection<string> availableSpeakers)
        {
            AvailableStates = availableStates;
            AvailableSpeakers = availableSpeakers;
            AvailableSounds = availableSounds;
            SetAvailableStates();
            SetAvailableSpeakers();
            LoadAvailableSoundsFromServer();
            SetAvailableSounds();
        }

        public ObservableCollection<string> AvailableStates { get; set; }

        public ObservableCollection<string> AvailableSounds { get; set; }

        public ObservableCollection<string> AvailableSpeakers { get; set; }

        public string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory() + @"\Sounds\"; }
        }

        void SetAvailableStates()
        {
            AvailableStates.Add("Тревога");
            AvailableStates.Add("Внимание");
            AvailableStates.Add("Неисправность");
            AvailableStates.Add("Требуется обслуживание");
            AvailableStates.Add("Отключено");
            AvailableStates.Add("Неизвестно");
            AvailableStates.Add("Норма(*)");
            AvailableStates.Add("Норма");
        }

        void SetAvailableSpeakers()
        {
            AvailableSpeakers.Add("<Нет>");
            AvailableSpeakers.Add("Тревога");
            AvailableSpeakers.Add("Внимание");
        }

        void SetAvailableSounds()
        {
            DirectoryInfo soundDirectory = Directory.CreateDirectory(CurrentDirectory);
            FileInfo[] files = soundDirectory.GetFiles();
            foreach (FileInfo fInfo in files)
            {
                AvailableSounds.Add(fInfo.Name);
            }
        }

        void LoadAvailableSoundsFromServer()
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
                    LoadFileFromServerToFilePath(kvp.Value, soundDirectory.FullName + kvp.Value);
                }
            }
        }

        void LoadFileFromServerToFilePath(string file, string destinationPath)
        {
            Stream stream = FiresecManager.GetFile(file);
            FileStream destinationStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);
            stream.CopyTo(destinationStream);
            destinationStream.Close();
        }

        Dictionary<string, string> GetFileHash()
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

        public RelayCommand LoadSoundCommand { get; private set; }
        void OnLoadSound()
        {
            LoadAvailableSoundsFromServer();
        }
    }
}
