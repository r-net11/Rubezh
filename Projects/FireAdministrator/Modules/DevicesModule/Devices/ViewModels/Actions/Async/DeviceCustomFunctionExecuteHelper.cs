using Controls.MessageBox;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

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

            ServiceFactory.ProgressService.Run(OnPropgress, OnlCompleted, device.PresentationAddressDriver + ". Выполнение функции");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.DeviceCustomFunctionExecute(_device.UID, _functionCode);
        }

        static void OnlCompleted()
        {
            if (_result == null)
            {
                MessageBoxService.Show("Ошибка при выполнении операции");
                return;
            }
            MessageBoxService.Show(_result);
        }
    }
}
