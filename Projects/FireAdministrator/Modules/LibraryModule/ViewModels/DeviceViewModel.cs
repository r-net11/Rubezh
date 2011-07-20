using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using DeviceControls;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel(LibraryViewModel parent, DeviceLibrary.Models.Device device)
        {
            Parent = parent;
            Driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == device.Id);
            SetStates(device);
            Initialize();
        }

        public DeviceViewModel(LibraryViewModel parent, FiresecClient.Models.Driver driver)
        {
            Parent = parent;
            Driver = driver;
            SetDefaultState();
            Initialize();
        }

        void Initialize()
        {
            DeviceControl = new DeviceControl(Id);
            AdditionalStates = new List<string>();

            AddStateCommand = new RelayCommand(OnAddState);
            AddAdditionalStateCommand = new RelayCommand(OnShowAdditionalStates);
            AddDeviceCommand = new RelayCommand(OnAddDevice);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
        }

        public FiresecClient.Models.Driver Driver { get; private set; }
        public LibraryViewModel Parent { get; private set; }
        public List<string> AdditionalStates;

        public string Id
        {
            get
            {
                return Driver.Id;
            }
        }

        public string IconPath
        {
            get
            {
                return Driver.ImageSource;
            }
        }

        public string Name
        {
            get
            {
                return Driver.DriverName;
            }
        }

        DeviceControl _deviceControl;
        public DeviceControl DeviceControl
        {
            get
            {
                return _deviceControl;
            }

            private set
            {
                _deviceControl = value;
                OnPropertyChanged("DeviceControl");
            }
        }

        ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get
            {
                return _states;
            }

            set
            {
                _states = value;
                OnPropertyChanged("States");
            }
        }

        StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get
            {
                return _selectedState;
            }

            set
            {
                _selectedState = value;
                if (value == null) return;
                if (_selectedState.Frames != null && _selectedState.Frames.Count > 0)
                {
                    _selectedState.SelectedFrame = _selectedState.Frames[0];
                }
                if (Parent.SelectedDevice != this)
                {
                    Parent.SelectedDevice = this;
                }
                UpdateDeviceControl(value);

                OnPropertyChanged("SelectedState");
            }
        }

        void SetDefaultState()
        {
            States = new ObservableCollection<StateViewModel>();
            States.Add(new StateViewModel("8", this));
        }

        void SetStates(DeviceLibrary.Models.Device device)
        {
            States = new ObservableCollection<StateViewModel>();
            foreach (var state in device.States)
            {
                States.Add(new StateViewModel(state, this));
            }
            States = new ObservableCollection<StateViewModel>(
                     from state in States
                     orderby state.Name
                     select state);
        }

        void UpdateDeviceControl(StateViewModel stateViewModel)
        {

            if (stateViewModel.IsAdditional)
            {
                DeviceControl.StateId = "-1";
                DeviceControl.AdditionalStates = new List<string>() { stateViewModel.Id };
            }
            else
            {
                DeviceControl.StateId = stateViewModel.Id;
                List<string> tmpAStates = new List<string>();
                foreach (var stateId in AdditionalStates)
                {
                    var state = States.FirstOrDefault(x => (x.Id == stateId) && (x.IsAdditional));
                    if (state.Class == SelectedState.Id)
                    {
                        tmpAStates.Add(state.Id);
                    }
                }
                DeviceControl.AdditionalStates = tmpAStates;
            }
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddState()
        {
            var addStateViewModel = new AddStateViewModel(this);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addStateViewModel))
            {
                States.Add(addStateViewModel.SelectedState);
            }
        }

        public RelayCommand AddAdditionalStateCommand { get; private set; }
        void OnShowAdditionalStates()
        {
            var addAdditionalStateViewModel = new AddAdditionalStateViewModel(this);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addAdditionalStateViewModel))
            {
                States.Add(addAdditionalStateViewModel.SelectedState);
            }
        }

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDevice()
        {
            var newDeviceViewModel = new AddDeviceViewModel(Parent);
            if (ServiceFactory.UserDialogs.ShowModalWindow(newDeviceViewModel))
            {
                Parent.Devices.Add(newDeviceViewModel.SelectedItem);
            }
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDevice()
        {
            var result = MessageBox.Show("Вы уверены что хотите удалить выбранное устройство?",
                                          "Окно подтверждения", MessageBoxButton.OKCancel,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                Parent.Devices.Remove(this);
            }
        }
    }
}
