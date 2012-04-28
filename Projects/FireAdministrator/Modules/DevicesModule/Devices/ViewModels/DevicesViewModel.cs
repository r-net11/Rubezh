using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevicesModule.Views;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public static DevicesViewModel Current { get; private set; }
        public DeviceCommandsViewModel DeviceCommandsViewModel { get; private set; }

        public DevicesViewModel()
        {
            Current = this;
            CopyCommand = new RelayCommand(OnCopy, CanCutCopy);
            CutCommand = new RelayCommand(OnCut, CanCutCopy);
            PasteCommand = new RelayCommand(OnPaste, CanPaste);
            PasteAsCommand = new RelayCommand(OnPasteAs, CanPasteAs);
            DeviceCommandsViewModel = new DeviceCommandsViewModel(this);

            BuildTree();
            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
        }

        #region DeviceSelection

        public List<DeviceViewModel> AllDevices;

        public void FillAllDevices()
        {
            AllDevices = new List<DeviceViewModel>();
            AddChildPlainDevices(Devices[0]);
        }

        void AddChildPlainDevices(DeviceViewModel parentViewModel)
        {
            AllDevices.Add(parentViewModel);
            foreach (var childViewModel in parentViewModel.Children)
            {
                AddChildPlainDevices(childViewModel);
            }
        }

        public void Select(Guid deviceUID)
        {
            FillAllDevices();

            var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (deviceViewModel != null)
                deviceViewModel.ExpantToThis();
            SelectedDevice = deviceViewModel;
        }

        #endregion

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
                _selectedDevice = value;
                if (value != null)
                    value.ExpantToThis();
                OnPropertyChanged("SelectedDevice");
            }
        }

        void BuildTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            AddDevice(FiresecManager.DeviceConfiguration.RootDevice, null);
        }

        public DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device, Devices);
            deviceViewModel.Parent = parentDeviceViewModel;

            var indexOf = Devices.IndexOf(parentDeviceViewModel);
            Devices.Insert(indexOf + 1, deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
                deviceViewModel.Children.Add(childDeviceViewModel);
            }

            return deviceViewModel;
        }

        public void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;
            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        public void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            if (parentDeviceViewModel.Device.Driver.Category != DeviceCategoryType.Device)
            {
                parentDeviceViewModel.IsExpanded = true;
                foreach (var deviceViewModel in parentDeviceViewModel.Children)
                {
                    ExpandChild(deviceViewModel);
                }
            }
        }

        Device _deviceToCopy;
        bool _isFullCopy;

        bool CanCutCopy()
        {
            return !(SelectedDevice == null || SelectedDevice.Parent == null ||
                SelectedDevice.Driver.IsAutoCreate || SelectedDevice.Parent.Driver.AutoChild == SelectedDevice.Driver.UID);
        }

        public RelayCommand CopyCommand { get; private set; }
        void OnCopy()
        {
            _deviceToCopy = SelectedDevice.Device.Copy(_isFullCopy = false);
        }

        public RelayCommand CutCommand { get; private set; }
        void OnCut()
        {
            _deviceToCopy = SelectedDevice.Device.Copy(_isFullCopy = true);
            SelectedDevice.RemoveCommand.Execute();

            FiresecManager.DeviceConfiguration.Update();
            ServiceFactory.SaveService.DevicesChanged = true;
            UpdateGuardVisibility();
        }

        bool CanPaste()
        {
            return (SelectedDevice != null && _deviceToCopy != null && SelectedDevice.Driver.AvaliableChildren.Contains(_deviceToCopy.Driver.UID));
        }

        public RelayCommand PasteCommand { get; private set; }
        void OnPaste()
        {
            var pasteDevice = _deviceToCopy.Copy(_isFullCopy);
            PasteDevice(pasteDevice);
        }

        bool CanPasteAs()
        {
            return (SelectedDevice != null && _deviceToCopy != null &&
                (DriversHelper.PanelDrivers.Contains(_deviceToCopy.Driver.DriverType) || DriversHelper.UsbPanelDrivers.Contains(_deviceToCopy.Driver.DriverType)) &&
                (SelectedDevice.Driver.DriverType == DriverType.Computer || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_1 || SelectedDevice.Driver.DriverType == DriverType.USB_Channel_2));
        }

        public RelayCommand PasteAsCommand { get; private set; }
        void OnPasteAs()
        {
            var pasteAsViewModel = new PasteAsViewModel(SelectedDevice.Driver.DriverType);
            if (ServiceFactory.UserDialogs.ShowModalWindow(pasteAsViewModel))
            {
                var pasteDevice = _deviceToCopy.Copy(_isFullCopy);

                pasteDevice.DriverUID = pasteAsViewModel.SelectedDriver.UID;
                pasteDevice.Driver = pasteAsViewModel.SelectedDriver;

                pasteDevice.SynchronizeChildern();

                PasteDevice(pasteDevice);
            }
        }

        void PasteDevice(Device device)
        {
            SelectedDevice.Device.Children.Add(device);
            device.Parent = SelectedDevice.Device;

            var newDevice = AddDevice(device, SelectedDevice);
            SelectedDevice.Children.Add(newDevice);
            CollapseChild(newDevice);

            FiresecManager.DeviceConfiguration.Update();
            ServiceFactory.SaveService.DevicesChanged = true;
            UpdateGuardVisibility();
        }

        public override void OnShow()
        {
            var devicesMenuViewModel = new DevicesMenuViewModel(this);
            ServiceFactory.Layout.ShowMenu(devicesMenuViewModel);

            if (DevicesMenuView.Current != null)
                DevicesMenuView.Current.AcceptKeyboard = true;
        }

        public override void OnHide()
        {
            ServiceFactory.Layout.ShowMenu(null);

            if (DevicesMenuView.Current != null)
                DevicesMenuView.Current.AcceptKeyboard = false;
        }

        public static void UpdateGuardVisibility()
        {
            var hasSecurityDevice = FiresecManager.DeviceConfiguration.Devices.Any(x => x.Driver.DeviceType == DeviceType.Sequrity);
            ServiceFactory.Events.GetEvent<GuardVisibilityChangedEvent>().Publish(hasSecurityDevice);
        }
    }
}