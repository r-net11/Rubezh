using Controls.MessageBox;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.Guard
{
    public class DeviceGetGuardUserListHelper
    {
        static Device _device;
        static string _guardUserList;

        public static void Run(Device device)
        {
            _device = device;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Чтение списка пользователей");
        }

        static void OnPropgress()
        {
            _guardUserList = FiresecManager.DeviceGetGuardUsersList(_device.UID);
        }

        static void OnCompleted()
        {
            if (_guardUserList == null)
            {
                MessageBoxService.Show("Ошибка при выполнении операции");
                return;
            }
            MessageBoxService.Show(_guardUserList);
        }
    }
}
