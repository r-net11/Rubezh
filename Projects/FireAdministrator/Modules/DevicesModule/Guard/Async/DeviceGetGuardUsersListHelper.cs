using Infrastructure.Common.MessageBox;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.Guard
{
    public class DeviceGetGuardUserListHelper
    {
        static Device _device;
        static OperationResult<string> _operationResult;

        public static void Run(Device device)
        {
            _device = device;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Чтение списка пользователей");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.DeviceGetGuardUsersList(_device.UID);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }
            MessageBoxService.Show(_operationResult.Result);
        }
    }
}
