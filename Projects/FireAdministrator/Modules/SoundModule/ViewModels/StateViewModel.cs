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
        public StateViewModel(string name, SoundPlayer soundPlayer,
            ObservableCollection<string> availableSounds,
            ObservableCollection<string> availableSpeakers)
        {
            Name = name;
            IsContinious = false;
            IsNowPlaying = false;
            SoundPlayer = soundPlayer;
            AvailableSounds = availableSounds;
            AvailableSpeakers = availableSpeakers;
            PlaySoundCommand = new RelayCommand(OnPlaySound);
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

        static bool _isNowPlaying;
        public bool IsNowPlaying
        {
            get
            {
                return _isNowPlaying;
            }

            set
            {
                _isNowPlaying = value;
                OnPropertyChanged("IsNowPlaying");
            }
        }

        string _currentDirectory;
        public string CurrentDirectory
        {
            get { return Directory.GetCurrentDirectory() + @"\Sounds\"; }
        }

        public ObservableCollection<string> AvailableSounds { get; private set; }

        public ObservableCollection<string> AvailableSpeakers { get; private set; }
        
        public SoundPlayer SoundPlayer { get; private set; }

        public void PlaySound()
        {
            SoundPlayer.SoundLocation = CurrentDirectory + Sound;
            if (!string.IsNullOrWhiteSpace(Sound))
            {
                SoundPlayer.Load();
            }
            else
            {
                IsNowPlaying = false;
                return;
            }
            if (SoundPlayer.IsLoadCompleted)
            {
                if (IsContinious)
                {
                    SoundPlayer.PlayLooping();
                }
                else
                {
                    SoundPlayer.Play();
                    IsNowPlaying = false;
                }
                
            }
        }

        public void StopPlaySound()
        {
            SoundPlayer.Stop();
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

        public void PlayPCSpeaker(object isContinious, object speaker)
        {
            
        }
    }
}
