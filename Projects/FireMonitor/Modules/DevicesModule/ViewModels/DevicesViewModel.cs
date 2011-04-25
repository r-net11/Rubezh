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
            Current = this;
            ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
        }

        void OnDeviceStateChanged(string path)
        {
            if (FiresecManager.CurrentStates.DeviceStates.Any(x => x.Path == path))
            {
                //DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == path);
                //if (plainDevices.Any(x => x.Device.Path == path))
                //{
                DeviceViewModel deviceViewModel = plainDevices.FirstOrDefault(x => x.Device.Path == path);
                    deviceViewModel.Update();
                    //deviceViewModel.MainState = deviceState.State;
                //}
            }
        }

        public void Initilize()
        {
            plainDevices = new List<DeviceViewModel>();
            Devices = new ObservableCollection<DeviceViewModel>();

            Device rooDevice = FiresecManager.CurrentConfiguration.RootDevice;

            DeviceViewModel rootDeviceViewModel = new DeviceViewModel();
            rootDeviceViewModel.Parent = null;
            rootDeviceViewModel.Initialize(rooDevice, Devices);
            //rootDeviceViewModel.IsExpanded = false;
            plainDevices.Add(rootDeviceViewModel);
            Devices.Add(rootDeviceViewModel);
            AddDevice(rooDevice, rootDeviceViewModel);

            ExpandChild(Devices[0]);

            FiresecManager.CurrentStates.DeviceStateChanged += new Action<string>(CurrentStates_DeviceStateChanged);
        }

        void CurrentStates_DeviceStateChanged(string path)
        {
            DeviceViewModel deviceViewModel = plainDevices.FirstOrDefault(x => x.Device.Path == path);

            deviceViewModel.UpdateParameters();
        }

        void AddDevice(Device parentDevice, DeviceViewModel parentDeviceViewModel)
        {
            foreach (Device device in parentDevice.Children)
            {
                DeviceViewModel deviceViewModel = new DeviceViewModel();
                deviceViewModel.Parent = parentDeviceViewModel;
                deviceViewModel.Initialize(device, Devices);
                //deviceViewModel.IsExpanded = true;
                parentDeviceViewModel.Children.Add(deviceViewModel);
                plainDevices.Add(deviceViewModel);
                Devices.Add(deviceViewModel);
                AddDevice(device, deviceViewModel);
            }
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

        public static DevicesViewModel Current { get; private set; }

        List<DeviceViewModel> plainDevices { get; set; }

        ObservableCollection<DeviceViewModel> devices;
        public ObservableCollection<DeviceViewModel> Devices
        {
            get { return devices; }
            set
            {
                devices = value;
                OnPropertyChanged("Devices");
            }
        }

        DeviceViewModel selectedDevice;
        public DeviceViewModel SelectedDevice
        {
            get { return selectedDevice; }
            set
            {
                selectedDevice = value;
                OnPropertyChanged("SelectedDevice");
            }
        }

        public void Select(string path)
        {
            if (string.IsNullOrEmpty(path) == false)
            {
                if (plainDevices.Any(x => x.Device.Path == path))
                {
                    DeviceViewModel deviceViewModel = plainDevices.FirstOrDefault(x => x.Device.Path == path);
                    deviceViewModel.ExpantToThis();
                    SelectedDevice = deviceViewModel;
                }
            }
        }

        public override void Dispose()
        {
        }
    }
}
