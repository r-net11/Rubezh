using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace LibraryModule.ViewModels
{
    public class LibraryViewModel : ViewPartViewModel
    {
        public LibraryViewModel()
        {
            AddDeviceCommand = new RelayCommand(OnAddDevice);
            RemoveDeviceCommand = new RelayCommand(OnRemoveDevice, CanRemoveDevice);
            AddStateCommand = new RelayCommand(OnAddState, CanAddState);
            RemoveStateCommand = new RelayCommand(OnRemoveState, CanRemoveState);
        }

        public void Initialize()
        {
            foreach (var libraryDevice in FiresecManager.DeviceLibraryConfiguration.Devices)
            {
                var driver = FiresecClient.FiresecManager.Drivers.First(x => x.UID == libraryDevice.DriverId);
                if(driver != null)
                {
                    libraryDevice.Driver = driver;
                }
                else
                {
                    Logger.Error("XLibraryViewModel.Initialize driver = null " + libraryDevice.DriverId.ToString());
                }
            }
            var devices = from LibraryDevice libraryDevice in FiresecManager.DeviceLibraryConfiguration.Devices orderby libraryDevice.Driver.DeviceClassName select libraryDevice;
            Devices = new ObservableCollection<DeviceViewModel>();
            foreach (var device in devices)
            {
                var deviceViewModel = new DeviceViewModel(device);
                Devices.Add(deviceViewModel);
            }
            SelectedDevice = Devices.FirstOrDefault();
        }

        ObservableCollection<DeviceViewModel> _devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return _devices; }
            set
            {
                _devices = value;
                OnPropertyChanged("Devices");
            }
        }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                var oldSelectedStateType = StateType.No;
                if (SelectedState != null)
                {
                    oldSelectedStateType = SelectedState.State.StateType;
                }
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");

                if (value != null)
                {
                    var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == SelectedDevice.LibraryDevice.DriverId);
                    States = new ObservableCollection<StateViewModel>();
                    var libraryStates = from LibraryState libraryState in SelectedDevice.LibraryDevice.States orderby libraryState.StateType descending select libraryState;
                    foreach (var libraryState in libraryStates)
                    {
                        var stateViewModel = new StateViewModel(libraryState, driver);
                        States.Add(stateViewModel);
                    }
                    SelectedState = States.FirstOrDefault(x => x.State.StateType == oldSelectedStateType);
                    if (SelectedState == null)
                        SelectedState = States.FirstOrDefault();
                }
                else
                {
                    SelectedState = null;
                }
            }
        }

        public RelayCommand AddDeviceCommand { get; private set; }
        void OnAddDevice()
        {
            var addDeviceViewModel = new DeviceDetailsViewModel();
            if (DialogService.ShowModalWindow(addDeviceViewModel))
            {
                FiresecManager.DeviceLibraryConfiguration.Devices.Add(addDeviceViewModel.SelectedDevice.LibraryDevice);
                Devices.Add(addDeviceViewModel.SelectedDevice);
                SelectedDevice = Devices.LastOrDefault();
                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        public RelayCommand RemoveDeviceCommand { get; private set; }
        void OnRemoveDevice()
        {
            FiresecManager.DeviceLibraryConfiguration.Devices.Remove(SelectedDevice.LibraryDevice);
            Devices.Remove(SelectedDevice);
            SelectedDevice = Devices.FirstOrDefault();
            ServiceFactory.SaveService.LibraryChanged = true;
        }
        bool CanRemoveDevice()
        {
            return SelectedDevice != null;
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
                OnPropertyChanged("DeviceControl");
            }
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddState()
        {
            var stateDetailsViewModel = new StateDetailsViewModel(SelectedDevice.LibraryDevice);
            if (DialogService.ShowModalWindow(stateDetailsViewModel))
            {
                SelectedDevice.LibraryDevice.States.Add(stateDetailsViewModel.SelectedState.State);
                States.Add(stateDetailsViewModel.SelectedState);
                SelectedState = States.LastOrDefault();
                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }
        bool CanAddState()
        {
            return SelectedDevice != null;
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveState()
        {
            SelectedDevice.LibraryDevice.States.Remove(SelectedState.State);
            States.Remove(SelectedState);
            SelectedState = States.FirstOrDefault();
            ServiceFactory.SaveService.LibraryChanged = true;
        }
        bool CanRemoveState()
        {
            return (SelectedState != null && SelectedState.State.StateType != StateType.No);
        }

        public DeviceControls.DeviceControl DeviceControl
        {
            get
            {
                if (SelectedDevice == null)
                    return null;
                if (SelectedState == null)
                    return null;

                var deviceControl = new DeviceControls.DeviceControl()
                {
                    DriverId = SelectedDevice.LibraryDevice.DriverId
                };
                deviceControl.StateType = SelectedState.State.StateType;

                var additionalStateCodes = new List<string>();
                if (SelectedState.IsAdditional)
                {
                    additionalStateCodes.Add(SelectedState.State.Code);
                    deviceControl.AdditionalStateCodes = additionalStateCodes;
                }

                deviceControl.Update();
                return deviceControl;
            }
        }

        public bool IsDebug
        {
            get { return ServiceFactory.AppSettings.IsDebug; }
        }
    }
}