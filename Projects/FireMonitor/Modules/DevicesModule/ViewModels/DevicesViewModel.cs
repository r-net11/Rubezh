using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Events;
using FiresecClient.Models;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        public void Initialize()
        {
            BuildDeviceTree();
            FiresecManager.States.DeviceStateChanged += new Action<string>(CurrentStates_DeviceStateChanged);
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
                OnPropertyChanged("SelectedDevice");
            }
        }

        void OnDeviceStateChanged(string id)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Id == id);
            if (deviceViewModel != null)
            {
                deviceViewModel.Update();
            }
        }

        void CurrentStates_DeviceStateChanged(string id)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Id == id);
            if (deviceViewModel != null)
            {
                deviceViewModel.UpdateParameters();
            }
        }

        void BuildDeviceTree()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            Device device = FiresecManager.Configuration.RootDevice;

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
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Id == id);
            if (deviceViewModel != null)
            {
                deviceViewModel.ExpantToThis();
                SelectedDevice = deviceViewModel;
            }
        }
    }
}
