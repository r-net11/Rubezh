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
                SoundsModule.HasChanged = true;
                OnPropertyChanged("SoundName");
            }
        }

        public BeeperType BeeperType
        {
            get { return Sound.BeeperType; }
            set 
            {
                Sound.BeeperType = value;
                SoundsModule.HasChanged = true;
                OnPropertyChanged("BeeperType");
            }
        }

        public bool IsContinious
        {
            get { return Sound.IsContinious; }
            set
            {
                Sound.IsContinious = value;
                SoundsModule.HasChanged = true;
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

        public List<BeeperType> AvailableSpeakers
        {
            get 
            {
                return new List<BeeperType>(Enum.GetValues(typeof(BeeperType)).OfType<BeeperType>());
            }
        }

        public const string DefaultName = "<нет>";
    }
}
