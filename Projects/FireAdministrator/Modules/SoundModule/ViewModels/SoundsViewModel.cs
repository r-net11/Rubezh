using System.Collections.ObjectModel;
using Infrastructure.Common;
using System.Media;
using System.Security.Cryptography;
using System.IO;

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
            DownloadHelper.UpdateSound();

            States = new ObservableCollection<SoundViewModel>();
            foreach (string str in DownloadHelper.GetAvailableStates) // Временно!
            {                                                         //
                States.Add(new SoundViewModel(str));
            }
            SelectedState = States[0];
        }

ObservableCollection<SoundViewModel> _states;
        public ObservableCollection<SoundViewModel> States
        {
            get { return _states; }
            set 
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        SoundViewModel _selectedState;
        public SoundViewModel SelectedState
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

        public RelayCommand PlaySoundCommand { get; private set; }
        void OnPlaySound()
        {
            SoundPlrHelper.PlaySound(SelectedState.SoundName, SelectedState.IsContinious);
        }
    }
}
