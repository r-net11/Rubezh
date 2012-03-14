using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common;

namespace DevicesModule.ViewModels
{
    public class DeviceConfigurationViewModel : DialogContent
    {
        Guid _deviceUID;
        DeviceConfiguration _deviceConfiguration;
        Device LocalRootDevice;
        Device RemoteRootDevice;

        public DeviceConfigurationViewModel(Guid deviceUID, DeviceConfiguration deviceConfiguration)
        {
            Title = "Сравнение конфигураций";
            ReplaceCommand = new RelayCommand(OnReplace);
            _deviceUID = deviceUID;
            _deviceConfiguration = deviceConfiguration;
            _deviceConfiguration.Update();
            foreach (var device in _deviceConfiguration.Devices)
            {
                device.Driver = FiresecManager.Drivers.FirstOrDefault(x=>x.UID == device.DriverUID);
            }

            LocalRootDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == _deviceUID);
            RemoteRootDevice = _deviceConfiguration.Devices.FirstOrDefault(x => x.UID == _deviceUID);
            LocalDevices = new DeviceTreeViewModel(LocalRootDevice, FiresecManager.DeviceConfiguration);
            RemoteDevices = new DeviceTreeViewModel(RemoteRootDevice, _deviceConfiguration);
        }

        public DeviceTreeViewModel LocalDevices { get; private set; }
        public DeviceTreeViewModel RemoteDevices { get; private set; }

        public RelayCommand ReplaceCommand { get; private set; }
        void OnReplace()
        {
            var parent = LocalRootDevice.Parent;
            parent.Children.Remove(LocalRootDevice);
            parent.Children.Add(RemoteRootDevice);
            RemoteRootDevice.Parent = parent;
            //LocalRootDevice = RemoteRootDevice;

            var deviceViewModel = DevicesViewModel.Current.Devices.FirstOrDefault(x => x.Device.UID == RemoteRootDevice.UID);
            DevicesViewModel.Current.CollapseChild(deviceViewModel);
            deviceViewModel.Children.Clear();
            foreach (var device in RemoteRootDevice.Children)
            {
                var childDeviceViewModel = DevicesViewModel.Current.AddDevice(device, deviceViewModel);
                childDeviceViewModel.Parent = deviceViewModel;
                deviceViewModel.Children.Add(childDeviceViewModel);
            }
            DevicesViewModel.Current.ExpandChild(deviceViewModel);

            FiresecManager.DeviceConfiguration.Update();
            ServiceFactory.SaveService.DevicesChanged = true;
            DevicesViewModel.UpdateGuardVisibility();

            Close(true);
        }
    }
}