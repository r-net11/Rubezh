using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Infrastructure.Common;
using System.Media;
using System.Security.Cryptography;
using System.IO;
using System;
using FiresecAPI.Models;
using Infrastructure.Common;
using Common;

namespace SoundsModule.ViewModels
{
    public class SoundsViewModel : RegionViewModel
    {
        public void Initialize()
        {
            IsNowPlaying = false;
            var sounds = FiresecClient.FiresecManager.SystemConfiguration.Sounds;
            if (sounds == null)
            {
                sounds = new List<Sound>();
            }

            Sounds = new ObservableCollection<SoundViewModel>();
            foreach (var statetype in Enum.GetValues(typeof(StateType)))
            {
                if ((StateType)statetype == StateType.No)
                    continue;
                var newSound = new Sound();
                newSound.StateType = (StateType)statetype;
                foreach (var sound in sounds)
                {
                    if (sound.StateType == newSound.StateType)
                    {
                        newSound = sound;
                    }
                }
                Sounds.Add(new SoundViewModel(newSound));
            }

            SelectedSound = Sounds[0];

            PlaySoundCommand = new RelayCommand(OnPlaySound);
        }

        public ObservableCollection<SoundViewModel> Sounds { get; private set; }
        
        SoundViewModel _selectedSound;
        public SoundViewModel SelectedSound
        {
            get { return _selectedSound; }
            set
            {
                _selectedSound = value;
                OnPropertyChanged("SelectedSound");
            }
        }

        bool _isNowPlaying;
        public bool IsNowPlaying
        {
            get { return _isNowPlaying; }
            set
            {
                _isNowPlaying = value;
                OnPropertyChanged("IsNowPlaying");
            }
        }

        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            if (IsNowPlaying)
            {
                string soundPath = FiresecClient.FiresecManager.FileHelper.GetSoundFilePath(SelectedSound.SoundName);
                AlarmPlayerHelper.Play(soundPath, SelectedSound.SpeakerType, SelectedSound.IsContinious);
                IsNowPlaying = SelectedSound.IsContinious;
            }
            else
            {
                AlarmPlayerHelper.Stop();
            }
        }

        public void Save()
        {
            FiresecClient.FiresecManager.SystemConfiguration.Sounds = new List<FiresecAPI.Models.Sound>();
            foreach (var sound in Sounds)
            {
                FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(sound.Sound);
            }
        }

        public override void OnHide()
        {
            IsNowPlaying = false;
            AlarmPlayerHelper.Stop();
        }
    }
}
