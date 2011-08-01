using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;
using System.Windows;
using System;
using System.Threading;

namespace SoundsModule.ViewModels
{
    public class StateViewModel : BaseViewModel
    {
        public StateViewModel(string name, SoundPlayer soundPlayer)
        {
            Name = name;
            IsContinious = false;
            IsNowPlaying = false;
            SoundPl = soundPlayer;
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

        public ObservableCollection<string> AvailableSounds
        {
            get
            {
                return new ObservableCollection<string>(){
                "<Нет>",
                "Sound1.wav",
                "Sound2.wav",
                "Sound3.wav",
                "Sound4.wav",
                "Sound5.wav",
                "Sound6.wav",
                "Sound7.wav",
                "Sound8.wav",
                "Sound9.wav"
                };
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

        SoundPlayer _soundPl;
        public SoundPlayer SoundPl
        {
            get
            {
                return _soundPl;
            }

            set
            {
                _soundPl = value;
            }
        }

        private static bool _isSoundPlayed;
        public bool IsSoundPlayed
        {
            get
            {
                return _isSoundPlayed;
            }

            set
            {
                _isSoundPlayed = value;
                OnPropertyChanged("IsSoundPlayed");
            }
        }

        public void PlaySound()
        {
            string path = @"Sounds/";
            SoundPl.SoundLocation = path + Sound;
            if (string.IsNullOrWhiteSpace(Sound))
            {
            }
            else
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
                    IsSoundPlayed = false;
                }
                
            }
        }

        public void StopPlaySound()
        {
            SoundPl.Stop();
        }


        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            if (IsSoundPlayed)
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
