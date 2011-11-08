using System;
using System.Linq;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class SynchronizeDeviceHelper
    {
        static Guid _deviceUID;
        static bool _isUsb;

        public static void Run(Guid deviceUID, bool isUsb)
        {
            _deviceUID = deviceUID;
            _isUsb = isUsb;

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == _deviceUID);
            AsyncOperationHelper.Run(OnPropgress, null, device.PresentationAddressDriver + ". Установка времени");
        }

        static void OnPropgress()
        {
            FiresecManager.SynchronizeDevice(_deviceUID, _isUsb);
        }
    }
}
