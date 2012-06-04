using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
    public static class DeviceCustomFunctionExecuteHelper
    {
        static Device _device;
        static string _functionCode;
        static OperationResult<string> _operationResult;

        public static void Run(Device device, string functionCode)
        {
            _device = device;
            _functionCode = functionCode;

            ServiceFactory.ProgressService.Run(OnPropgress, OnlCompleted, device.PresentationAddressDriver + ". Выполнение функции");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.DeviceCustomFunctionExecute(_device.UID, _functionCode);
        }

        static void OnlCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }
            var result = _operationResult.Result;
            result = result.Replace("[OK]", "");
            MessageBoxService.Show(result);
        }
    }
}