using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class DeviceCustomFunctionExecuteHelper
    {
        static Device _device;
        static string _functionCode;
        static string _result;

        public static void Run(Device device, string functionCode)
        {
            _device = device;
            _functionCode = functionCode;

            AsyncOperationHelper.Run(OnPropgress, OnlCompleted, device.PresentationAddressDriver + ". Выполнение функции");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.DeviceCustomFunctionExecute(_device.UID, _functionCode);
        }

        static void OnlCompleted()
        {
            if (_result == null)
            {
                DialogBox.DialogBox.Show("Ошибка при выполнении операции");
                return;
            }
            DialogBox.DialogBox.Show(_result);
        }
    }
}
