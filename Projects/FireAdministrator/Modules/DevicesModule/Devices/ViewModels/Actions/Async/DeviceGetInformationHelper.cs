using Controls.MessageBox;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

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
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Чтение информации об устройстве");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.DeviceGetInformation(_device.UID, _isUsb);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }
            ServiceFactory.UserDialogs.ShowModalWindow(new DeviceDescriptionViewModel(_device.UID, _operationResult.Result));
        }
    }
}