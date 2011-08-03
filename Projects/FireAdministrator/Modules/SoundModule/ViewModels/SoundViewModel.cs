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

        public SoundViewModel(string stateName)
        {
            Sound = new FiresecAPI.Models.Sound();
            StateName = stateName;
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

        public string StateName
        {
            get
            {
                if (AvailableStates.IndexOf(Sound.StateName) != -1)
                {
                    return Sound.StateName;
                }
                else
                {
                    return DownloadHelper.DefaultName;
                }
            }
            set
            {
                Sound.StateName = value;
                OnPropertyChanged("StateName");
            }
        }

        public string SoundName
        {
            get
            {
                if (AvailableSounds.IndexOf(Sound.SoundName) != -1)
                {
                    return Sound.SoundName;
                }
                else
                {
                    return DownloadHelper.DefaultName;
                }
            }
            set
            {
                Sound.SoundName = value;
                OnPropertyChanged("SoundName");
            }
        }

        public string SpeakerName
        {
            get
            {
                if (AvailableSpeakers.IndexOf(Sound.SpeakerName) != -1)
                {
                    return Sound.SpeakerName;
                }
                else
                {
                    return DownloadHelper.DefaultName;
                }
            }
            set
            {
                Sound.SpeakerName = value;
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
            get { return DownloadHelper.GetAvailableStates; }
        }

        public ObservableCollection<string> AvailableSounds 
        {
            get { return DownloadHelper.GetAvailableSounds; } 
        }

        public ObservableCollection<string> AvailableSpeakers
        {
            get { return DownloadHelper.GetAvailableSpeakers; } 
        }
    }
}
