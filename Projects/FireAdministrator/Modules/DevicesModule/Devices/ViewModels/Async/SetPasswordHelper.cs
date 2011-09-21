using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class SetPasswordHelper
    {
        static Guid _deviceUID;
        static DevicePasswordType _devicePasswordType;
        static string _password;

        public static void Run(Guid deviceUID, DevicePasswordType devicePasswordType, string password)
        {
            _deviceUID = deviceUID;
            _devicePasswordType = devicePasswordType;
            _password = password;

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x=>x.UID == _deviceUID);
            AsyncOperationHelper.Run(OnPropgress, null, device.PresentationAddressDriver + ". Установка пароля");
        }

        static void OnPropgress()
        {
            FiresecManager.SetPassword(_deviceUID, _devicePasswordType, _password);
        }
    }
}
