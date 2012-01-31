using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace LibraryModule.ViewModels
{
    public class DeviceViewModel : BaseViewModel
    {
        public static Driver UnknownDeviceDriver
        {
            get
            {
                return new Driver()
                {
                    ImageSource = FileHelper.GetIconFilePath("Unknown_Device") + ".ico",
                    UID = new Guid("00000000-0000-0000-0000-000000000000"),
                    Name = "Неизвестное устройство",
                    States = new List<DriverState>(),
                };
            }
        }

        public Driver Driver { get; private set; }

        public LibraryDevice LibraryDevice { get; private set; }

        public Guid Id
        {
            get { return LibraryDevice.DriverId; }
        }

        public DeviceViewModel(LibraryDevice libraryDevice)
        {
            LibraryDevice = libraryDevice;
            Driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID == LibraryDevice.DriverId);
            if (Driver == null)
            {
                Driver = UnknownDeviceDriver;
                LibraryDevice.DriverId = Driver.UID;
                LibraryDevice.States = null;
                StateViewModels = new ObservableCollection<StateViewModel>();
            }
            else
            {
                if (LibraryDevice.States == null) SetDefaultStateTo(LibraryDevice);

                StateViewModels = new ObservableCollection<StateViewModel>(
                    LibraryDevice.States.Select(state => new StateViewModel(state, Driver))
                );
            }

            AddStateCommand = new RelayCommand(OnAddState);
            AddAdditionalStateCommand = new RelayCommand(OnAddAdditionalState);
            RemoveStateCommand = new RelayCommand(OnRemoveState, CanRemoveState);
        }

        public ObservableCollection<StateViewModel> StateViewModels { get; private set; }

        StateViewModel _selectedStateViewModel;
        public StateViewModel SelectedStateViewModel
        {
            get { return _selectedStateViewModel; }
            set
            {
                _selectedStateViewModel = value;
                OnPropertyChanged("SelectedStateViewModel");
                OnPropertyChanged("DeviceControl");
            }
        }

        public DeviceControls.DeviceControl DeviceControl
        {
            get
            {
                if (SelectedStateViewModel == null)
                    return null;

                var deviceControl = new DeviceControls.DeviceControl();
                deviceControl.DriverId = LibraryDevice.DriverId;

                var additionalStateCodes = new List<string>();
                if (SelectedStateViewModel.IsAdditional)
                {
                    additionalStateCodes.Add(SelectedStateViewModel.State.Code);
                    deviceControl.AdditionalStateCodes = additionalStateCodes;

                    return deviceControl;
                }

                deviceControl.StateType = SelectedStateViewModel.State.StateType;
                foreach (var stateViewModel in StateViewModels)
                {
                    if (stateViewModel.IsAdditional &&
                        stateViewModel.IsChecked &&
                        stateViewModel.State.StateType == SelectedStateViewModel.State.StateType
                    ) additionalStateCodes.Add(stateViewModel.State.Code);
                }
                deviceControl.AdditionalStateCodes = additionalStateCodes;

                return deviceControl;
            }
        }

        public static void SetDefaultStateTo(LibraryDevice device)
        {
            device.States = new List<LibraryState>();
            device.States.Add(StateViewModel.GetDefaultStateWith());
        }

        public static LibraryDevice GetDefaultDriverWith(Guid driverId)
        {
            var device = new LibraryDevice();
            device.DriverId = driverId;
            SetDefaultStateTo(device);

            return device;
        }

        public RelayCommand AddStateCommand { get; private set; }
        void OnAddState()
        {
            var addStateViewModel = new StateDetailsViewModel(LibraryDevice);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addStateViewModel))
            {
                LibraryDevice.States.Add(addStateViewModel.SelectedItem.State);
                StateViewModels.Add(addStateViewModel.SelectedItem);
                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        public RelayCommand AddAdditionalStateCommand { get; private set; }
        void OnAddAdditionalState()
        {
            var addAdditionalStateViewModel = new AdditionalStateDetailsViewModel(LibraryDevice);
            if (ServiceFactory.UserDialogs.ShowModalWindow(addAdditionalStateViewModel))
            {
                LibraryDevice.States.Add(addAdditionalStateViewModel.SelectedItem.State);
                StateViewModels.Add(addAdditionalStateViewModel.SelectedItem);
                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        public RelayCommand RemoveStateCommand { get; private set; }
        void OnRemoveState()
        {
            var dialogResult = DialogBox.DialogBox.ShowQuestion("Удалить выбранное состояние?");

            if (dialogResult == MessageBoxResult.Yes)
            {
                LibraryDevice.States.Remove(SelectedStateViewModel.State);
                StateViewModels.Remove(SelectedStateViewModel);
                ServiceFactory.SaveService.LibraryChanged = true;
            }
        }

        bool CanRemoveState()
        {
            return SelectedStateViewModel != null && SelectedStateViewModel.State.StateType != StateViewModel.DefaultStateType;
        }
    }
}