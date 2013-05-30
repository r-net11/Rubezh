using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
    public static class FS2SynchronizeDeviceHelper
    {
		static Device _device;
        static bool _isUsb;
        static OperationResult<bool> _operationResult;

        public static void Run(Device device, bool isUsb)
        {
			_device = device;
            _isUsb = isUsb;

			var fs2ProgressService = new FS2ProgressService();
			fs2ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndName + ". Установка времени");
        }

        static void OnPropgress()
        {
			_operationResult = FiresecManager.FS2ClientContract.DeviceDatetimeSync(_device.UID, _isUsb);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                return;
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}
