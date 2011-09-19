using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public class AutodetectionViewModel : DialogContent
    {
        public void Initialize(DeviceConfiguration autodetectedDeviceConfiguration)
        {
            Devices = new List<AutoDetectedDeviceViewModel>();
            var root = AddDevice(autodetectedDeviceConfiguration.RootDevice, null);
            Devices.Add(root);
        }

        AutoDetectedDeviceViewModel AddDevice(Device device, AutoDetectedDeviceViewModel parentDeviceViewModel)
        {
            var deviceViewModel = new AutoDetectedDeviceViewModel(device);

            foreach (var childDevice in device.Children)
            {
                var childDeviceViewModel = AddDevice(childDevice, deviceViewModel);
                deviceViewModel.Children.Add(childDeviceViewModel);
            }

            return deviceViewModel;
        }

        public List<AutoDetectedDeviceViewModel> Devices { get; set; }
    }
}
