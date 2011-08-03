using FiresecClient;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;
using System.Windows;
using System;
using System.Threading;
using System.IO;
using System.Collections.Generic;

namespace SoundsModule.ViewModels
{
    public class SoundViewModel : RegionViewModel
    {
        public SoundViewModel(string stateName)
        {
            StateName = stateName;
            SoundName = DownloadHelper.DefaultName;
            SpeakerName = DownloadHelper.DefaultName;
            IsContinious = DownloadHelper.DefaultIsContinious;
        }

        string _stateName;
        public string StateName
        {
            get 
            {
                if (AvailableStates.IndexOf(_stateName) != -1)
                {
                    return _stateName;
                }
                else
                {
                    return DownloadHelper.DefaultName;
                }
            }
            set
            {
                _stateName = value;
                OnPropertyChanged("StateName");
            }
        }

        string _soundName;
        public string SoundName
        {
            get 
            {
                if (AvailableSounds.IndexOf(_soundName) != -1)
                {
                    return _soundName;
                }
                else
                {
                    return DownloadHelper.DefaultName;
                }
            }
            set
            {
                _soundName = value;
                OnPropertyChanged("SoundName");
            }
        }

        string _speakerName;
        public string SpeakerName
        {
            get 
            {
                if (AvailableSpeakers.IndexOf(_speakerName) != -1)
                {
                    return _speakerName;
                }
                else
                {
                    return DownloadHelper.DefaultName;
                }
            }
            set
            {
                _speakerName = value;
                OnPropertyChanged("SpeakerName");
            }
        }

        bool _isContinious;
        public bool IsContinious
        {
            get { return _isContinious; }
            set
            {
                _isContinious = value;
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
