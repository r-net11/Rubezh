using Infrastructure.Common.MessageBox;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class DeviceWriteConfigurationHelper
    {
        static Device _device;
        static bool _isUsb;
        static OperationResult<bool> _operationResult;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Запись конфигурации в устройство");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.DeviceWriteConfiguration(_device.UID, _isUsb);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}