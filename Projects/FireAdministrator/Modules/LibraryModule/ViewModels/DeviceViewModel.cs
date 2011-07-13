using System;
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
        public DeviceViewModel()
        {
            Current = this;
            DeviceControl = new DeviceControl();
            ShowDevicesCommand = new RelayCommand(OnShowDevices);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice);
            ShowAdditionalStatesCommand = new RelayCommand(OnShowAdditionalStates);
            ShowStatesCommand = new RelayCommand(OnShowStates);
            AdditionalStates = new List<string>();
            States = new ObservableCollection<StateViewModel>();
        }

        public static DeviceViewModel Current { get; private set; }

        public void Initialize()
        {
            States = new ObservableCollection<StateViewModel>();
            var stateViewModel = new StateViewModel();
            stateViewModel.Id = Convert.ToString(8);
            var frameViewModel = new FrameViewModel(Helper.EmptyFrame, 300, 0);
            stateViewModel.Frames = new ObservableCollection<FrameViewModel>() { frameViewModel };
            States = new ObservableCollection<StateViewModel>(){stateViewModel};
            LibraryViewModel.Current.Update();
        }

        private DeviceControl _deviceControl;
        public DeviceControl DeviceControl
        {
            get { return _deviceControl; }
            set
            {
                _deviceControl = value;
                OnPropertyChanged("DeviceControl");
            }
        }

        public string IconPath
        {
            get
            {
                var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x=>x.Id == Id);
                return driver.ImageSource;
            }
        }

        private string  _id;
        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        public string Name
        {
            get
            {
                var driver = FiresecManager.Configuration.Drivers.FirstOrDefault(x => x.Id == Id);
                return driver.DriverName;
            }
        }

        private StateViewModel _selectedState;
        public StateViewModel SelectedState
        {
            get { return _selectedState; }
            set
            {
                if (value == null)
                    return;
                _selectedState = value;
                LibraryViewModel.Current.SelectedState = _selectedState;
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

        public List<string> AdditionalStates;

        public RelayCommand ShowDevicesCommand { get; private set; }
        private static void OnShowDevices()
        {
            var devicesListViewModel = new NewDeviceViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(devicesListViewModel);
        }

        public RelayCommand ShowStatesCommand { get; private set; }
        public static void OnShowStates()
        {
            var statesListViewModel = new NewStateViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(statesListViewModel);
        }

        public RelayCommand ShowAdditionalStatesCommand { get; private set; }
        public static void OnShowAdditionalStates()
        {
            var additionalStatesListViewModel = new NewAdditionalStateViewModel();
            ServiceFactory.UserDialogs.ShowModalWindow(additionalStatesListViewModel);
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        private void OnRemoveDevice()
        {
            var result = MessageBox.Show("Вы уверены что хотите удалить выбранное устройство?",
                                          "Окно подтверждения", MessageBoxButton.OKCancel,
                                          MessageBoxImage.Question);
            if (result == MessageBoxResult.Cancel) return;

            LibraryViewModel.Current.Devices.Remove(this);
            LibraryViewModel.Current.SelectedState = null;
            LibraryViewModel.Current.Update();
        }
    }
}
