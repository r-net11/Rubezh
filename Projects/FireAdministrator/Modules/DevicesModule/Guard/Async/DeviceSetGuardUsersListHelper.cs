using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.MessageBox;

namespace DevicesModule.Guard
{
    public class DeviceSetGuardUsersListHelper
    {
        static Device _device;
        static OperationResult<bool> _operationResult;
        static string _users;

        public static void Run(Device device, string users)
        {
            _device = device;
            _users = users;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Запись списка пользователей");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.DeviceSetGuardUsersList(_device.UID, _users);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}
