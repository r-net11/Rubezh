using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public DeviceViewModel(LibraryViewModel parent, DeviceLibrary.Models.Device device)
        {
            ParentLibrary = parent;
            Driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == device.Id);
            SetStates(device);

            Initialize();
        }

        public DeviceViewModel(LibraryViewModel parent, Driver driver)
        {
            ParentLibrary = parent;
            Driver = driver;
            SetDefaultState();

            Initialize();
        }

        void Initialize()
        {
            AddStateCommand = new RelayCommand(OnAddState);
            AddAdditionalStateCommand = new RelayCommand(OnShowAdditionalStates);
            AddDeviceCommand = new RelayCommand(OnAddDevice);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
        }

        public Driver Driver { get; private set; }
        public LibraryViewModel ParentLibrary { get; private set; }

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

        ObservableCollection<StateViewModel> _states;
        public ObservableCollection<StateViewModel> States
        {
            get
            {
                return _states;
            }

            private set
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
                if (value != null && value.Frames != null && value.Frames.Count > 0)
                {
                    value.SelectedFrame = value.Frames[0];
                }

                if (ParentLibrary.SelectedDevice != this)
                {
                    ParentLibrary.SelectedDevice = this;
                }

                OnPropertyChanged("SelectedState");
            }
        }

        void SetDefaultState()
        {
            States = new ObservableCollection<StateViewModel>();
            States.Add(new StateViewModel(StateViewModel.defaultClassId, this));
        }

        void SetStates(DeviceLibrary.Models.Device device)
        {
            var states = new ObservableCollection<StateViewModel>();
            foreach (var state in device.States)
            {
                states.Add(new StateViewModel(state, this));
            }
            States = states;
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddState()
        {
            var addStateViewModel = new AddStateViewModel(this);
            addStateViewModel.Initialize();

            if (ServiceFactory.UserDialogs.ShowModalWindow(addStateViewModel))
            {
                States.Add(addStateViewModel.SelectedItem);
            }
        }

        public RelayCommand AddAdditionalStateCommand { get; private set; }
        void OnShowAdditionalStates()
        {
            var addAdditionalStateViewModel = new AddAdditionalStateViewModel(this);
            addAdditionalStateViewModel.Initialize();

            if (ServiceFactory.UserDialogs.ShowModalWindow(addAdditionalStateViewModel))
            {
                States.Add(addAdditionalStateViewModel.SelectedItem);
            }
        }

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDevice()
        {
            var addDeviceViewModel = new AddDeviceViewModel(ParentLibrary);
            addDeviceViewModel.Initialize();

            if (ServiceFactory.UserDialogs.ShowModalWindow(addDeviceViewModel))
            {
                ParentLibrary.Devices.Add(addDeviceViewModel.SelectedItem);
            }
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDevice()
        {
            var result = MessageBox.Show("Вы уверены что хотите удалить выбранное устройство?",
                                          "Окно подтверждения",
                                          MessageBoxButton.OKCancel,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                ParentLibrary.Devices.Remove(this);
            }
        }
    }
}
