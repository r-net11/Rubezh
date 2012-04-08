using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Controls.MessageBox;

namespace DevicesModule.ViewModels
{
    public static class DeviceWriteConfigurationHelper
    {
        static Device _device;
        static bool _isUsb;
        static string _result;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Запись конфигурации в устройство");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.DeviceWriteConfiguration(_device.UID, _isUsb);
        }

        static void OnCompleted()
        {
            if (string.IsNullOrEmpty(_result))
                MessageBoxService.Show("Операция закончилась успешно");
            else
                MessageBoxService.Show(_result);
        }
    }
}