using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace SoundsModule.ViewModels
{
    public class SoundsViewModel : RegionViewModel
    {
        public SoundsViewModel()
        {
        }

        public void Initialize()
        {
            DownloadHelper.UpdateSound();
            IsNowPlaying = false;
            var sysConfSounds = FiresecClient.FiresecManager.SystemConfiguration.Sounds;
            Sounds = new ObservableCollection<SoundViewModel>();

            if ((sysConfSounds != null) && (sysConfSounds.Count > 0))
            {
                foreach (var sound in sysConfSounds)
                {
                    Sounds.Add(new SoundViewModel(sound));
                }
            }
            else
            {
                foreach (var statetype in Enum.GetValues(typeof(StateType)))
                {
                    if ((StateType) statetype == StateType.No)
                        continue;
                    Sound newSound = new Sound();
                    newSound.StateType = (StateType) statetype;
                    Sounds.Add(new SoundViewModel(newSound));
                }
            }

            SelectedSound = Sounds[0];

            PlaySoundCommand = new RelayCommand(OnPlaySound);
        }

        ObservableCollection<SoundViewModel> _sounds;
        public ObservableCollection<SoundViewModel> Sounds
        {
            get { return _sounds; }
            set
            {
                _sounds = value;
                OnPropertyChanged("Sounds");
            }
        }

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

        public void Save()
        {
            if (Sounds != null)
            {
                FiresecClient.FiresecManager.SystemConfiguration.Sounds = new List<FiresecAPI.Models.Sound>();
                foreach (var sound in Sounds)
                {
                    FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(sound.Sound);
                }
            }
        }

        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            if (IsNowPlaying)
            {
                SoundPlayerHelper.PlaySound(SelectedSound.SoundName, SelectedSound.IsContinious);
                SoundPlayerHelper.PlayPCSpeaker(SelectedSound.SpeakerType, SelectedSound.IsContinious);
                if (!SelectedSound.IsContinious)
                {
                    IsNowPlaying = false;
                }
            }
            else
            {
                SoundPlayerHelper.StopPlaySound();
                SoundPlayerHelper.StopPlayPCSpeaker();
            }
        }

        public override void OnHide()
        {
            base.OnHide();
            IsNowPlaying = false;
            SoundPlayerHelper.StopPlaySound();
            SoundPlayerHelper.StopPlayPCSpeaker();
        }
    }
}