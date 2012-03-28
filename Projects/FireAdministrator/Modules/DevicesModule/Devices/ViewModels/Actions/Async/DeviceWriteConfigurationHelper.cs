using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

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
            ServiceFactory.ProgressService.Run(OnPropgress, null, _device.PresentationAddressDriver + ". Запись конфигурации в устройство");
        }

        static void OnPropgress()
        {
            FiresecManager.DeviceWriteConfiguration(_device.UID, _isUsb);
        }
    }
}