using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.IO;
using FiresecClient;

namespace SoundsModule.ViewModels
{
    class SoundsLoadViewModel
    {
        public SoundsLoadViewModel()
        {

        }

        public Inicialized(ObservableCollection<string> availableStates,
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

        //List<string> _hashListSoundFiles;
        //public List<string> HashListSoundFiles
        //{
        //    get 
        //    {
        //        return _hashListSoundFiles;
        //    }

        //    set 
        //    {
        //        _hashListSoundFiles = value;
        //    }
        //}

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
            AvailableStates.Add("<Нет>");
            AvailableStates.Add("Тревога");
            AvailableStates.Add("Внимание");
        }


        void LoadAvailableSoundsFromServer()
        {
            if (Directory.Exists(CurrentDirectory))
            {
                var listSounds = Directory.GetFiles(CurrentDirectory);
                List<string> HashListSoundFiles = new List<string>();
                foreach (string str in listSounds)
                {    
                    HashListSoundFiles.Add(GetFileHash());
                }
            }
            else
            {
                var listSounds = FiresecManager.GetSoundsFileName();
                Stream fileStream;
                DirectoryInfo newDirectory = Directory.CreateDirectory(CurrentDirectory);
                foreach (string file in listSounds)
                {
                    Stream stream = FiresecManager.GetFile(file);
                    FileStream destinationStream = new FileStream(newDirectory.FullName + file, FileMode.Create, FileAccess.Write);
                    stream.CopyTo(destinationStream);
                    destinationStream.Close();
                    AvailableSounds.Add(file);
                }
            }
        }

        string GetFileHash(string filepath)
        {
            byte[] hash;
            using (Stream fs = File.OpenRead(filepath))
            {
                hash = MD5.Create().ComputeHash(fs);
            }
        }
    }
}
