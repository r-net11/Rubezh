using System.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    public class SoundViewModel : BaseViewModel
    {
        public SoundViewModel(Sound sound)
        {
            Sound = sound;
        }

        Sound _sound;
        public Sound Sound
        {
            get { return _sound; }
            set
            {
                _sound = value;
            }
        }

        public StateType StateType
        {
            get { return Sound.StateType; }
        }

        public string SoundName
        {
            get
            {
                return Sound.SoundName;
            }
            set
            {
                Sound.SoundName = value;
                OnPropertyChanged("SoundName");
            }
        }

        public SpeakerType SpeakerType
        {
            get { return Sound.SpeakerType; }
            set 
            {
                Sound.SpeakerType = value; 
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

        public List<string> AvailableSounds 
        {
            get
            {
                List<string> fileNames = new List<string>();
                fileNames.Add(DownloadHelper.DefaultName);
                foreach (string str in Directory.GetFiles(DownloadHelper.CurrentDirectory))
                {
                    fileNames.Add(Path.GetFileName(str));
                }
                return fileNames;
            }
        }

        public Array AvailableSpeakers
        {
            get { return Enum.GetValues(typeof(SpeakerType)); }
        }

    }
}
