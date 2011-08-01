using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;
using System.Windows;
using System;
using System.Threading;
using System.IO;

namespace SoundsModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public StateViewModel(string name, SoundPlayer soundPlayer)
        {
            Name = name;
            IsContinious = false;
            IsNowPlaying = false;
            SoundPlayer = soundPlayer;
            PlaySoundCommand = new RelayCommand(OnPlaySound);
            LoadSoundCommand = new RelayCommand(OnLoadSounds);
        }

        public void Inicialized()
        {

        }
        string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }

        string _sound;
        public string Sound
        {
            get
            {
                return _sound;
            }

            set
            {
                _sound = value;
                OnPropertyChanged("Sound");
            }
        }

        string _speaker;
        public string Speaker
        {
            get { return _speaker; }
            set
            {
                _speaker = value;
                OnPropertyChanged("Speaker");
            }
        }

        bool _isContinious;
        public bool IsContinious
        {
            get 
            { 
                return _isContinious;
            }

            set
            {
                _isContinious = value;
                OnPropertyChanged("IsContinious");
            }
        }

        bool _isNowPlaying;
        public bool IsNowPlaying
        {
            get
            {
                return _isNowPlaying;
            }

            set
            {
                _isNowPlaying = value;
            }
        }

        string _currentDirectory;
        public string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory() + @"\Sounds\"; }
        }

        public ObservableCollection<string> AvailableSounds
        {
            get
            {
                return _availableSounds;
            }
        }

        public ObservableCollection<string> AvailableSpeakers
        {
            get
            {
                return new ObservableCollection<string>(){
                "<Нет>",
                "Тревога",
                "Внимание"
                };
            }
        }

        public SoundPlayer SoundPlayer { get; private set; }

        public void PlaySound()
        {
            SoundPl.SoundLocation = CurrentDirectory + Sound;
            if (!string.IsNullOrWhiteSpace(Sound))
            {
                SoundPl.Load();
            }
            if (SoundPl.IsLoadCompleted)
            {
                if (IsContinious)
                {
                    SoundPl.PlayLooping();
                }
                else
                {
                    SoundPl.Play();
                    IsNowPlaying = false;
                }
                
            }
        }

        public void StopPlaySound()
        {
            SoundPl.Stop();
        }

        public RelayCommand LoadSoundCommand { get; private set; }
        public void OnLoadSounds()
        {
            
        }

        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            if (IsNowPlaying)
            {
                StopPlaySound();
                PlaySound();
            }
            else
            {
                StopPlaySound();
            }
        }

        static void PlayPCSpeaker(object isContinious, object speaker)
        {
            
        }
    }
}
