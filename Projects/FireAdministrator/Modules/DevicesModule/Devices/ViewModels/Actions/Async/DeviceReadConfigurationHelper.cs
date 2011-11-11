using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class DeviceReadConfigurationHelper
    {
        static Device _device;
        static bool _isUsb;
        static DeviceConfiguration _deviceConfiguration;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;
            AsyncOperationHelper.Run(OnPropgress, OnlCompleted, _device.PresentationAddressDriver + ". Чтение конфигурации из устройства");
        }

        static void OnPropgress()
        {
            _deviceConfiguration = FiresecManager.DeviceReadConfiguration(_device.UID, _isUsb);
        }

        static void OnlCompleted()
        {
            //ServiceFactory.UserDialogs.ShowModalWindow(new DeviceDescriptionViewModel(_device.UID, _description));
        }
    }
}
