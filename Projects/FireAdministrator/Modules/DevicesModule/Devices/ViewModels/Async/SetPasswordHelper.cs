using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace DevicesModule.ViewModels
{
    public static class SetPasswordHelper
    {
        static Guid _deviceUID;
        static bool _isUsb;
        static DevicePasswordType _devicePasswordType;
        static string _password;

        public static void Run(Guid deviceUID, bool isUsb, DevicePasswordType devicePasswordType, string password)
        {
            _deviceUID = deviceUID;
            _isUsb = isUsb;
            _devicePasswordType = devicePasswordType;
            _password = password;

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x=>x.UID == _deviceUID);
            AsyncOperationHelper.Run(OnPropgress, null, device.PresentationAddressDriver + ". Установка пароля");
        }

        static void OnPropgress()
        {
            FiresecManager.SetPassword(_deviceUID, _isUsb, _devicePasswordType, _password);
        }
    }
}
