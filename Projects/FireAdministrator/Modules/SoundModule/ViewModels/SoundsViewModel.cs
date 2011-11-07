using System;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure;

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
                var newSound = new Sound() { StateType = stateType };
                if (FiresecClient.FiresecManager.SystemConfiguration.Sounds.IsNotNullOrEmpty())
                {
                    var sound = FiresecClient.FiresecManager.SystemConfiguration.Sounds.FirstOrDefault(x => x.StateType == stateType);
                    if (sound == null)
                        FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(newSound);
                    else
                        newSound = sound;
                }
                else
                {
                    FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(newSound);
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

        bool CanPlaySound()
        {
            return ((IsNowPlaying) || (SelectedSound != null &&
                    ((string.IsNullOrEmpty(SelectedSound.SoundName) == false) ||
                    SelectedSound.BeeperType != BeeperType.None)));
        }

        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            if (IsNowPlaying == false)
            {
                AlarmPlayerHelper.Play(FiresecClient.FileHelper.GetSoundFilePath(SelectedSound.SoundName), SelectedSound.BeeperType, SelectedSound.IsContinious);
                IsNowPlaying = SelectedSound.IsContinious;
            }
            else
            {
                AlarmPlayerHelper.Stop();
                IsNowPlaying = false;
            }
        }

        public override void OnShow()
        {
            var soundsMenuViewModel = new SoundsMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(soundsMenuViewModel);
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);
            IsNowPlaying = false;
            AlarmPlayerHelper.Stop();
        }
    }
}