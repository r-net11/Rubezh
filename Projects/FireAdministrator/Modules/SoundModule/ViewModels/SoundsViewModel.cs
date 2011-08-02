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
            SoundPl = new SoundPlayer();
            AvailableStates = new ObservableCollection<string>();
            AvailableSounds = new ObservableCollection<string>();
            AvailableSpeakers = new ObservableCollection<string>();
            LoadSoundsData = new SoundsLoadViewModel();
            LoadSoundsData.Inicialized(AvailableStates, AvailableSounds, AvailableSpeakers);

            States = new ObservableCollection<StateViewModel>();
            foreach (string str in AvailableStates)
            {
                States.Add(new StateViewModel(str, SoundPl, AvailableSounds, AvailableSpeakers));
            }
            SelectedState = States[0];
        }

        private SoundsLoadViewModel _loadSoundsData;
        public SoundsLoadViewModel LoadSoundsData
        {
            get 
            {
                return _loadSoundsData; 
            }

            set 
            {
                _loadSoundsData = value;
            }
        }

        private ObservableCollection<string> _availableStates;
        public ObservableCollection<string> AvailableStates
        {
            get
            {
                return _availableStates;
            }
            set
            {
                _availableStates = value;
            }
        }

        private ObservableCollection<string> _availableSounds;
        public ObservableCollection<string> AvailableSounds
        {
            get
            {
                return _availableSounds;
            }
            set
            {
                _availableSounds = value;
            }
        }
        
        private ObservableCollection<string> _availableSpeakers;
        public ObservableCollection<string> AvailableSpeakers
        {
            get
            {
                return _availableSpeakers;
            }
            set
            {
                _availableSpeakers = value;
            }
        }

        ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get { return _states; }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged("SelectedState");
            }
        }

        SoundPlayer _soundPl;
        public SoundPlayer SoundPl
        {
            get 
            {
                return _soundPl; 
            }

            set 
            {
                _soundPl = value;
            }
        }
    }
}
