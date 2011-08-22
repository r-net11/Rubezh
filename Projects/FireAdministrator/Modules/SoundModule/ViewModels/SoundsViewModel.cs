using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure;
using System.Windows;

namespace SoundsModule.ViewModels
{
    public class SoundsViewModel : RegionViewModel
    {
        public SoundsViewModel()
        {
            PlaySoundCommand = new RelayCommand(OnPlaySound, CanPlaySound);
        }
        public void Inicialize()
        {
            IsNowPlaying = false;

            Sounds = new ObservableCollection<SoundViewModel>();
            foreach (StateType stateType in Enum.GetValues(typeof(StateType)))
            {
                if (stateType == StateType.No)
                    continue;

                var newSound = new Sound();
                newSound.StateType = stateType;
                if (FiresecClient.FiresecManager.SystemConfiguration.Sounds.IsNotNullOrEmpty())
                {
                    foreach (var sound in FiresecClient.FiresecManager.SystemConfiguration.Sounds)
                    {
                        if (sound.StateType == newSound.StateType)
                        {
                            newSound = sound;
                        }
                    }
                }
                Sounds.Add(new SoundViewModel(newSound));
            }

            SelectedSound = Sounds[0];
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

        bool CanPlaySound(object obj)
        {
            return ((IsNowPlaying)||(SelectedSound != null && 
                ((string.IsNullOrEmpty(SelectedSound.SoundName) == false) || 
                SelectedSound.BeeperType != BeeperType.None)));
        }

        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            if (IsNowPlaying == false)
            {
                string soundPath = FiresecClient.FileHelper.GetSoundFilePath(SelectedSound.SoundName);
                AlarmPlayerHelper.Play(soundPath, SelectedSound.BeeperType, SelectedSound.IsContinious);
                IsNowPlaying = SelectedSound.IsContinious;
            }
            else
            {
                AlarmPlayerHelper.Stop();
                IsNowPlaying = false;
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
