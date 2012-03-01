using FiresecAPI.Models;
using FiresecClient;
using Controls.MessageBox;
using Infrastructure;

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
            ServiceFactory.ProgressService.Run(OnPropgress, OnlCompleted, _device.PresentationAddressDriver + ". Чтение конфигурации из устройства");
        }

        static void OnPropgress()
        {
            _deviceConfiguration = FiresecManager.DeviceReadConfiguration(_device.UID, _isUsb);
        }

        static void OnlCompleted()
        {
            if (_deviceConfiguration == null)
            {
                MessageBoxService.Show("Ошибка при выполнении операции");
                return;
            }
            ServiceFactory.UserDialogs.ShowModalWindow(new DeviceConfigurationViewModel(_device.UID, _deviceConfiguration));
        }
    }
}