using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Events;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }
        private string _avc;

        public string Avc
        {
            get { return _avc; }
            set { _avc = value; }
        }
        public void Initialize()
        {
            BuildDeviceTree();
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChangedEvent);
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

        void OnDeviceStateChanged(string id)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.Id == id);
            if (deviceViewModel != null)
            {
                deviceViewModel.Update();
            }
        }

        void OnDeviceStateChangedEvent(string id)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.Id == id);
            if (deviceViewModel != null)
            {
                deviceViewModel.UpdateParameters();
            }
        }

        void BuildDeviceTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            Device device = FiresecManager.DeviceConfiguration.RootDevice;

            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Parent = null;
            deviceViewModel.Initialize(device, Devices);
            deviceViewModel.IsExpanded = true;
            Devices.Add(deviceViewModel);
            AddDevice(device, deviceViewModel);

            if (Devices.Count > 0)
            {
                SelectedDevice = Devices[0];
            }
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (var device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, Devices);
                deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                Devices.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }

        public void Select(string id)
        {
            SelectedDevice = Devices.FirstOrDefault(x => x.Device.Id == id);
        }
    }
}
