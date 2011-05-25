using System.Linq;
using Firesec.Metadata;
using Infrastructure.Common;
using System.Collections.ObjectModel;
using DeviceLibrary;

namespace LibraryModule.ViewModels
{
    internal class NewAdditionalStateViewModel : DialogContent
    {
        public NewAdditionalStateViewModel()
        {
            Title = "Список дополнительных состояний";
            _selectedDevice = LibraryViewModel.Current.SelectedDevice;
            _driver = LibraryManager.Drivers.FirstOrDefault(x => x.id == _selectedDevice.Id);
            Initialize();
            AddCommand = new RelayCommand(OnAdd);
            CancelCommand = new RelayCommand(OnCancel);
        }

        private readonly DeviceViewModel _selectedDevice;
        private readonly drvType _driver;

        private bool _isEnabled;
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
            foreach (var item in _driver.state)
            {
                if (_selectedDevice.States.FirstOrDefault(x => (x.Id == item.id) && (x.IsAdditional)) != null) continue;
                var frames = new ObservableCollection<FrameViewModel> { new FrameViewModel(Helper.EmptyFrame, 300, 0) };
                var stateViewModel = new StateViewModel(item.id, _selectedDevice, true, frames);
                States.Add(stateViewModel);
            }
            States = new ObservableCollection<StateViewModel>(States.OrderBy(x => x.Class));
        }

        public RelayCommand AddCommand { get; private set; }
        private void OnAdd()
        {
            if (SelectedState == null) return;
            _selectedDevice.States.Add(SelectedState);
            _selectedDevice.States = new ObservableCollection<StateViewModel>(_selectedDevice.States.OrderByDescending(x=>x.Name));
            States.Remove(SelectedState);
            _selectedDevice.SortStates();
            LibraryViewModel.Current.Update();
            IsEnabled = false;
            Close(true);
        }

        public RelayCommand CancelCommand { get; private set; }
        private void OnCancel()
        {
            Close(false);
        }
    }
}
