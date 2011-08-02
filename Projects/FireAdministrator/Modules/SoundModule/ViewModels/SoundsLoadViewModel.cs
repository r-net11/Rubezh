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

        void LoadAvailableSoundsFromServer()
        {
            if (Directory.Exists(CurrentDirectory))
            {
                var listSounds = Directory.GetFiles(CurrentDirectory);
                foreach (string file in listSounds)
                {
                    AvailableSounds.Add(Path.GetFileName(file));
                }
                //List<string> HashListSoundFiles = new List<string>();
                //foreach (string str in listSounds)
                //{    
                //    HashListSoundFiles.Add(GetFileHash());
                //}
            }
            else
            {
                var listSounds = FiresecManager.GetSoundsFileName();
                Stream fileStream;
                DirectoryInfo newDirectory = Directory.CreateDirectory(CurrentDirectory);
                foreach (string file in listSounds)
                {
                    LoadFileFromServerToFilePath(file, newDirectory.FullName + file);
                    AvailableSounds.Add(file);
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

        byte[] GetFileHash(string filepath)
        {
            byte[] hash;
            using (Stream fs = File.OpenRead(filepath))
            {
                hash = MD5.Create().ComputeHash(fs);
            }
            return hash;
        }

        public RelayCommand LoadSoundCommand { get; private set; }
        void OnLoadSound()
        {
            LoadAvailableSoundsFromServer();
        }
    }
}
