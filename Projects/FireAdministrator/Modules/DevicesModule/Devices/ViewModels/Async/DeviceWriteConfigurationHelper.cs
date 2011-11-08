using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class DeviceWriteConfigurationHelper
    {
        static Device _device;
        static bool _isUsb;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;
            AsyncOperationHelper.Run(OnPropgress, null, _device.PresentationAddressDriver + ". Запись конфигурации в устройство");
        }

        static void OnPropgress()
        {
            FiresecManager.DeviceWriteConfiguration(_device.UID, _isUsb);
        }
    }
}
