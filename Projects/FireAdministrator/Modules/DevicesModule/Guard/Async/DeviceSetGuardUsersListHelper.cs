using Controls.MessageBox;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.Guard
{
    public class DeviceSetGuardUsersListHelper
    {
        static Device _device;
        static string _result;
        static string _users;

        public static void Run(Device device, string users)
        {
            _device = device;
            _users = users;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Запись списка пользователей");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.DeviceSetGuardUsersList(_device.UID, _users);
        }

        static void OnCompleted()
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
