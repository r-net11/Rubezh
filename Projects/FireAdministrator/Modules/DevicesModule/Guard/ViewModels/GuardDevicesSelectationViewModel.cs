using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Common;
using DevicesModule.ViewModels;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class GuardDevicesSelectationViewModel : SaveCancelDialogContent
    {
        public GuardUser GuardUser { get; private set; }

        public GuardDevicesSelectationViewModel(GuardUser guardUser)
        {
            AddOneCommand = new RelayCommand(OnAddOne, CanAddOne);
            RemoveOneCommand = new RelayCommand(OnRemoveOne, CanRemoveOne);
            AddAllCommand = new RelayCommand(OnAddAll, CanAddAll);
            RemoveAllCommand = new RelayCommand(OnRemoveAll, CanRemoveAll);

            Title = "Выберите охранное устройство";
            GuardUser = guardUser;

            DeviceList = new List<Guid>(GuardUser.Devices);
            Devices = new ObservableCollection<DeviceViewModel>();
            AvailableDevices = new ObservableCollection<DeviceViewModel>();

            UpdateDevices();
        }

        void UpdateDevices()
        {
            var availableDevices = new HashSet<Device>();
            var devices = new HashSet<Device>();

            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if ((device.Driver.DriverType == DriverType.Rubezh_2OP) || (device.Driver.DriverType == DriverType.USB_Rubezh_2OP))
                {
                    if (DeviceList.Contains(device.UID))
                    {
                        device.AllParents.ForEach(x => { devices.Add(x); });
                        devices.Add(device);
                    }
                    else
                    {
                        device.AllParents.ForEach(x => { availableDevices.Add(x); });
                        availableDevices.Add(device);
                    }
                }
            }

            Devices.Clear();
            BuildTree(devices, Devices);

            AvailableDevices.Clear();
            BuildTree(availableDevices, AvailableDevices);

            if (Devices.IsNotNullOrEmpty())
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }
            else
            {
                SelectedDevice = null;
            }

            if (AvailableDevices.IsNotNullOrEmpty())
            {
                CollapseChild(AvailableDevices[0]);
                ExpandChild(AvailableDevices[0]);
                SelectedAvailableDevice = AvailableDevices[0];
            }
            else
            {
                SelectedAvailableDevice = null;
            }
        }

        void BuildTree(HashSet<Device> hashSetDevices, ObservableCollection<DeviceViewModel> devices)
        {
            foreach (var device in hashSetDevices)
            {
                var deviceViewModel = new DeviceViewModel(device, devices);
                deviceViewModel.IsExpanded = true;
                if ((device.Driver.DriverType == DriverType.Rubezh_2OP) || (device.Driver.DriverType == DriverType.USB_Rubezh_2OP))
                    deviceViewModel.IsBold = true;
                devices.Add(deviceViewModel);
            }

            foreach (var device in devices)
            {
                if (device.Device.Parent != null)
                {
                    var parent = devices.FirstOrDefault(x => x.Device.UID == device.Device.Parent.UID);
                    device.Parent = parent;
                    parent.Children.Add(device);
                }
            }
        }

        void CollapseChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = false;

            foreach (var deviceViewModel in parentDeviceViewModel.Children)
            {
                CollapseChild(deviceViewModel);
            }
        }

        void ExpandChild(DeviceViewModel parentDeviceViewModel)
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

        public List<Guid> DeviceList { get; set; }
        public ObservableCollection<DeviceViewModel> AvailableDevices { get; set; }
        public ObservableCollection<DeviceViewModel> Devices { get; set; }

        DeviceViewModel _selectedAvailableDevice;
        public DeviceViewModel SelectedAvailableDevice
        {
            get { return _selectedAvailableDevice; }
            set
            {
                _selectedAvailableDevice = value;
                OnPropertyChanged("SelectedAvailableDevice");
            }
        }

        DeviceViewModel _selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return _selectedDevice; }
            set
            {
                _selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public bool CanAddOne()
        {
            return ((SelectedAvailableDevice != null) && (SelectedAvailableDevice.IsBold));
        }

        public bool CanAddAll()
        {
            return (AvailableDevices.IsNotNullOrEmpty());
        }

        public bool CanRemoveOne()
        {
            return ((SelectedDevice != null) && (SelectedDevice.IsBold));
        }

        public bool CanRemoveAll()
        {
            return (Devices.IsNotNullOrEmpty());
        }

        public RelayCommand AddOneCommand { get; private set; }
        void OnAddOne()
        {
            DeviceList.Add(SelectedAvailableDevice.UID);
            UpdateDevices();
        }

        public RelayCommand AddAllCommand { get; private set; }
        void OnAddAll()
        {
            foreach (var deviceViewModel in AvailableDevices)
            {
                if (deviceViewModel.IsBold)
                    DeviceList.Add(deviceViewModel.UID);
            }
            UpdateDevices();
        }

        public RelayCommand RemoveAllCommand { get; private set; }
        void OnRemoveAll()
        {
            DeviceList.Clear();
            UpdateDevices();
        }

        public RelayCommand RemoveOneCommand { get; private set; }
        void OnRemoveOne()
        {
            DeviceList.Remove(SelectedDevice.UID);
            UpdateDevices();
        }

        protected override void Save(ref bool cancel)
        {
            GuardUser.Devices = DeviceList;
        }
    }
}
