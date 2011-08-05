using System.Collections.Generic;
using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;
using System;

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
            var sounds = new ObservableCollection<SoundViewModel>();
            var sysConfSounds = FiresecClient.FiresecManager.SystemConfiguration.Sounds;
            //**************************
            //временно(заменю на Linq)
            bool isContains = false;
            ObservableCollection<string> availableStates = new ObservableCollection<string>();
            foreach (var id in FiresecAPI.StateTypeConverter.ConvertStateTypeToListString())
            {
                availableStates.Add(id);
            }

            foreach (var state in availableStates)
            {
                foreach (var sound in sysConfSounds)
                {
                    if (string.Equals(sound.StateType, state))
                    {
                        isContains = true;
                        sounds.Add(new SoundViewModel(sound));
                    }
                }
                if (!isContains)
                {
                    sounds.Add(new SoundViewModel(state));
                }
                else
                {
                    isContains = false;
                }
            }
            //********************
            Sounds = sounds;
            SelectedSound = Sounds[0];

            PlaySoundCommand = new RelayCommand(OnPlaySound);
            SaveCommand = new RelayCommand(Save);

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

        public RelayCommand SaveCommand { get; private set; }
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
                SoundPlayerHelper.PlayPCSpeaker(SelectedSound.SpeakerName, SelectedSound.IsContinious);
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
    }
}
