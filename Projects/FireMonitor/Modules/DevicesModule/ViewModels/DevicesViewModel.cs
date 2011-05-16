using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure;
using System.Collections.ObjectModel;
using FiresecClient;
using Infrastructure.Events;

namespace DevicesModule.ViewModels
{
    public class DevicesViewModel : RegionViewModel
    {
        public DevicesViewModel()
        {
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        void OnDeviceStateChanged(string id)
        {
            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Id == id))
            {
                DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.Id == id);
                deviceViewModel.Update();
            }
        }

        public void Initialize()
        {
            Devices = new ObservableCollection<DeviceViewModel>();

            Device device = FiresecManager.CurrentConfiguration.RootDevice;

            DeviceViewModel deviceViewModel = new DeviceViewModel();
            deviceViewModel.Parent = null;
            deviceViewModel.Initialize(device, Devices);
            Devices.Add(deviceViewModel);
            AddDevice(device, deviceViewModel);

            ExpandChild(Devices[0]);

            FiresecManager.CurrentStates.DeviceStateChanged += new Action<string>(CurrentStates_DeviceStateChanged);
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, Devices);
                parentDeviceViewModel.Children.Add(deviceViewModel);
                Devices.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
        }

        void CurrentStates_DeviceStateChanged(string id)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.Id == id);

            deviceViewModel.UpdateParameters();
        }

        void ExpandChild(DeviceViewModel parentDeviceViewModel)
        {
            parentDeviceViewModel.IsExpanded = true;
            foreach (DeviceViewModel deviceViewModel in parentDeviceViewModel.Children)
            {
                deviceViewModel.IsExpanded = true;
                ExpandChild(deviceViewModel);
            }
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

        public void Select(string id)
        {
            DeviceViewModel deviceViewModel = Devices.FirstOrDefault(x => x.Device.Id == id);
            if (deviceViewModel != null)
            {
                deviceViewModel.ExpantToThis();
                SelectedDevice = deviceViewModel;
            }
        }

        public override void Dispose()
        {
        }
    }
}
