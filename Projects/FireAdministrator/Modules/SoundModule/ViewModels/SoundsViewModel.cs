using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace SoundsModule.ViewModels
{
    public class SoundsViewModel : RegionViewModel
    {
        public SoundsViewModel()
        {
            
        }

        public void Initialize()
        {
            SoundPlrHelper = new SoundPlayerHelper();
            PlaySoundCommand = new RelayCommand(OnPlaySound);
            SaveCommand = new RelayCommand(Save);
            DownloadHelper.UpdateSound();

            Sounds = Load();
            if (Sounds.Count == 0)
            {
                Sounds = new ObservableCollection<SoundViewModel>();
                foreach (string str in DownloadHelper.GetAvailableStates) // Временно!
                {                                                         //
                    Sounds.Add(new SoundViewModel(str));
                }
            }
            
            SelectedSound = Sounds[0];
        }
        
        ObservableCollection<SoundViewModel> _sounds;
        public ObservableCollection<SoundViewModel> Sounds
        {
            get { return _sounds; }
            set 
            {
                _sounds = value;
                OnPropertyChanged("States");
            }
        }

        SoundViewModel _selectedState;
        public SoundViewModel SelectedSound
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }

        SoundPlayerHelper _soundPlrHelper;
        public SoundPlayerHelper SoundPlrHelper { get; set; }

        public RelayCommand SaveCommand { get; private set; }
        public void Save()
        {
            if (Sounds != null)
            {
                FiresecClient.FiresecManager.SystemConfiguration.Sounds = new List<FiresecAPI.Models.Sound>();
                foreach (var state in Sounds)
                {
                    FiresecClient.FiresecManager.SystemConfiguration.Sounds.Add(state.Sound);
                }
            }
        }

        public ObservableCollection<SoundViewModel> Load()
        {
            var sounds = new ObservableCollection<SoundViewModel>();
            var sysConfSounds = FiresecClient.FiresecManager.SystemConfiguration.Sounds;
            if (sysConfSounds != null)
            {
                foreach (var sound in sysConfSounds)
                {
                    sounds.Add(new SoundViewModel(sound));
                }
            }
            return sounds;
        }
        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            SoundPlrHelper.PlaySound(SelectedSound.SoundName, SelectedSound.IsContinious);
        }
    }
}
