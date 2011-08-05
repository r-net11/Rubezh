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
                if (value == Enum.GetName(typeof(DownloadHelper.AvailableSpeaker), 0))
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

        public List<string> AvailableStates
        {
            get { return FiresecAPI.StateTypeConverter.ConvertStateTypeToListString(); }
        }

        public ObservableCollection<string> AvailableSounds 
        {
            get
            {
                ObservableCollection<string> fileNames = new ObservableCollection<string>();
                fileNames.Add(DownloadHelper.DefaultName);
                foreach (string str in Directory.GetFiles(DownloadHelper.CurrentDirectory))
                {
                    fileNames.Add(Path.GetFileName(str));
                }
                return fileNames;
            } 
        }

        public ObservableCollection<string> AvailableSpeakers 
        {
            get
            {
                ObservableCollection<string> speakerNames = new ObservableCollection<string>();
                foreach (var speakername in Enum.GetNames(typeof(DownloadHelper.AvailableSpeaker)))
                {
                    speakerNames.Add(speakername);
                }
                return speakerNames;
            }
        }
    }
}
