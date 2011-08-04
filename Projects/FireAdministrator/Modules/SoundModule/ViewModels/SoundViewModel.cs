using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;
using System.Windows;
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using FiresecAPI.Models;

namespace SoundsModule.ViewModels
{
    public class SoundViewModel : RegionViewModel
    {
        public SoundViewModel(Sound sound)
        {
            Sound = sound;
        }

        public SoundViewModel(string stateType)
        {
            Sound = new Sound();
            StateType = stateType;
            SoundName = DownloadHelper.DefaultName;
            SpeakerName = DownloadHelper.DefaultName;
            IsContinious = DownloadHelper.DefaultIsContinious;
        }

        Sound _sound;
        public Sound Sound
        {
            get { return _sound; }
            set
            {
                _sound = value;
                //OnPropertyChanged("Sound");
            }
        }

        public string StateType
        {
            get
            {
                if ((string.IsNullOrWhiteSpace(Sound.StateType)) || (AvailableStates.IndexOf(Sound.StateType) == -1))
                {
                    return DownloadHelper.DefaultName;
                }
                else
                {
                    return Sound.StateType;
                }
            }
            set
            {
                Sound.StateType = value;
                OnPropertyChanged("StateName");
            }
        }

        public string SoundName
        {
            get
            {
                if ((string.IsNullOrWhiteSpace(Sound.SoundName)) || (AvailableSounds.IndexOf(Sound.SoundName) == -1))
                {
                    return DownloadHelper.DefaultName;
                }
                else
                {
                    return Sound.SoundName;
                }
            }
            set
            {
                if (value == DownloadHelper.DefaultName)
                {
                    Sound.SoundName = null;
                }
                else
                {
                    Sound.SoundName = value;
                }
                OnPropertyChanged("SoundName");
            }
        }

        public string SpeakerName
        {
            get
            {
                if ((string.IsNullOrWhiteSpace(Sound.SpeakerName)) || (AvailableSpeakers.IndexOf(Sound.SpeakerName) == -1))
                {
                    return DownloadHelper.DefaultName;
                }
                else
                {
                    return Sound.SpeakerName;
                }
            }
            set
            {
                if (value == DownloadHelper.DefaultName)
                {
                    Sound.SpeakerName = null;
                }
                else
                {
                    Sound.SpeakerName = value;
                }
                OnPropertyChanged("SpeakerName");
            }
        }

        public bool IsContinious
        {
            get { return Sound.IsContinious; }
            set
            {
                Sound.IsContinious = value;
                OnPropertyChanged("IsContinious");
            }
        }

        public ObservableCollection<string> AvailableStates
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

        public ObservableCollection<string> AvailableSounds 
        {
            get { return DownloadHelper.GetAvailableSounds; } 
        }

        public ObservableCollection<string> AvailableSpeakers
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
    }
}
