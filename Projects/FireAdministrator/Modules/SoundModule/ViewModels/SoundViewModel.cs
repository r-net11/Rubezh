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
            get { return Sound.SoundName; }
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
                var listSounds = new List<string>();
                listSounds.Add("<нет>");
                listSounds.AddRange(FiresecClient.FiresecManager.FileHelper.GetListSounds);
                return listSounds;
            }
        }

        public Array AvailableSpeakers
        {
            get { return Enum.GetValues(typeof(SpeakerType)); }
        }

        public string SoundFilePath
        {
            get { return FiresecClient.FiresecManager.FileHelper.GetFilePath(SoundName); }
        }
    }
}
