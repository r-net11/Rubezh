using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
    public static class DeviceGetInformationHelper
    {
        static Device _device;
        static bool _isUsb;
        static OperationResult<string> _operationResult;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressAndDriver + ". Чтение информации об устройстве");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.DeviceGetInformation(_device.UID, _isUsb);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                return;
            }
			DialogService.ShowModalWindow(new DeviceDescriptionViewModel(_device.UID, _operationResult.Result));
        }
    }
}