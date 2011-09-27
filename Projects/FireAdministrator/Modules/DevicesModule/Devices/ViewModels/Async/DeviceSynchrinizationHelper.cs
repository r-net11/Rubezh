using System;
using System.Linq;
using DevicesModule.ViewModels;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class DeviceSynchrinizationHelper
    {
        static Guid _deviceUID;

        public static void Run(Guid deviceUID)
        {
            _deviceUID = deviceUID;

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == _deviceUID);
            AsyncOperationHelper.Run(OnPropgress, null, device.PresentationAddressDriver + ". Установка времени");
        }

        static void OnPropgress()
        {
            FiresecManager.SynchronizeDevice(_deviceUID);
        }
    }
}
