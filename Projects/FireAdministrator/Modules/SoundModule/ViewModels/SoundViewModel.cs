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

        public Sound Sound { get; set; }

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
                OnPropertyChanged("SpeakerType");
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
                listSounds.Add(DefaultName); //TODO переделать на null!)
                listSounds.AddRange(FiresecClient.FiresecManager.FileHelper.SoundsList);
                return listSounds;
            }
        }

        public List<SpeakerType> AvailableSpeakers
        {
            get 
            {
                return new List<SpeakerType>(Enum.GetValues(typeof(SpeakerType)).OfType<SpeakerType>());
            }
        }

        public const string DefaultName = "<нет>";
    }
}
