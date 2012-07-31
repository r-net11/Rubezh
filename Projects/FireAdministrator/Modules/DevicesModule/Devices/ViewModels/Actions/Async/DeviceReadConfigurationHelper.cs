using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
    public static class DeviceReadConfigurationHelper
    {
        static Device _device;
        static bool _isUsb;
        static OperationResult<DeviceConfiguration> _operationResult;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;
            ServiceFactory.ProgressService.Run(OnPropgress, OnlCompleted, _device.PresentationAddressAndDriver + ". Чтение конфигурации из устройства");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.DeviceReadConfiguration(_device.UID, _isUsb);
        }

        static void OnlCompleted()
        {
            if (_operationResult.HasError)
            {
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                return;
            }
            DialogService.ShowModalWindow(new DeviceConfigurationViewModel(_device.UID, _operationResult.Result));
        }
    }
}