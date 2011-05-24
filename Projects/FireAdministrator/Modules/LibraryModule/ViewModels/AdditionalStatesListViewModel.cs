using System.Linq;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceLibrary;

namespace LibraryModule.ViewModels
{
    internal class AdditionalStatesListViewModel : DialogContent
    {
        public AdditionalStatesListViewModel()
        {
            Title = "Список дополнительных состояний";
            _selectedDevice = LibraryViewModel.Current.SelectedDevice;
            Initialize();
            AddCommand = new RelayCommand(OnAdd);
        }

        private readonly DeviceViewModel _selectedDevice;

        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                if (value == null) {IsEnabled = false; return;}
                IsEnabled = _selectedDevice.States.FirstOrDefault(x => (x.Id == value.Class) && (!x.IsAdditional)) != null;
                OnPropertyChanged("SelectedState");
            }
        }

        private ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get { return _states; }
            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        public void Initialize()
        {
            States = new ObservableCollection<StateViewModel>();
            var driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == _selectedDevice.Id);
            foreach (var item in driver.state)
            {
                if (_selectedDevice.States.FirstOrDefault(x => (x.Id == item.id) && (x.IsAdditional)) != null) continue;
                var frames = new ObservableCollection<FrameViewModel> { new FrameViewModel(Helper.EmptyFrame, 300, 0) };
                var stateViewModel = new StateViewModel(item.id, _selectedDevice, true, frames);
                States.Add(stateViewModel);
            }
        }

        public RelayCommand AddCommand { get; private set; }
        private void OnAdd()
        {
            if (SelectedState == null) return;
            _selectedDevice.States.Add(SelectedState);
            States.Remove(SelectedState);
            _selectedDevice.SortStates();
            LibraryViewModel.Current.Update();
        }
    }
}
