using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += OnDeviceStateChanged;
        }

        public void Initialize()
        {
            BuildDeviceTree();
            if (Devices.Count > 0)
            {
                CollapseChild(Devices[0]);
                ExpandChild(Devices[0]);
                SelectedDevice = Devices[0];
            }

            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<Guid>(OnDeviceStateChangedEvent);
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
                _selectedDevice = value;
                if (value != null)
                {
                    value.ExpantToThis();
                }
                OnPropertyChanged("SelectedDevice");
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

        void OnDeviceStateChanged(Guid deviceUID)
        {
            var deviceViewModel = Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (deviceViewModel != null)
            {
                deviceViewModel.Update();
            }
        }

        void OnDeviceStateChangedEvent(Guid deviceUID)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (deviceViewModel != null)
            {
                deviceViewModel.UpdateParameters();
            }
        }

        void BuildDeviceTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();
            AllDevices = new List<DeviceViewModel>();
            var device = FiresecManager.DeviceConfiguration.RootDevice;
            AddDevice(device, null);
        }

        DeviceViewModel AddDevice(Device device, DeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new DeviceViewModel(device, Devices);
            deviceViewModel.Parent = parentDeviceViewModel;
            AllDevices.Add(deviceViewModel);

            var indexOf = Devices.IndexOf(parentDeviceViewModel);
            Devices.Insert(indexOf + 1, deviceViewModel);

            foreach (var childDevice in device.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
                deviceViewModel.Children.Add(childDeviceViewModel);
            }

            return deviceViewModel;
        }

        #region DeviceSelection

        public List<DeviceViewModel> AllDevices;

        public void Select(Guid deviceUID)
        {
            var deviceViewModel = AllDevices.FirstOrDefault(x => x.Device.UID == deviceUID);
            if (deviceViewModel != null)
            {
                deviceViewModel.ExpantToThis();
                SelectedDevice = deviceViewModel;
            }
        }

        #endregion
    }
}