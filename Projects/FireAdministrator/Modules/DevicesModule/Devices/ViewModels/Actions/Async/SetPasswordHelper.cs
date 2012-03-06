using System;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Controls.MessageBox;

namespace DevicesModule.ViewModels
{
    public static class SetPasswordHelper
    {
        static Guid _deviceUID;
        static bool _isUsb;
        static DevicePasswordType _devicePasswordType;
        static string _password;
        static bool _result;

        public static void Run(Guid deviceUID, bool isUsb, DevicePasswordType devicePasswordType, string password)
        {
            _deviceUID = deviceUID;
            _isUsb = isUsb;
            _devicePasswordType = devicePasswordType;
            _password = password;

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == _deviceUID);
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressDriver + ". Установка пароля");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.SetPassword(_deviceUID, _isUsb, _devicePasswordType, _password);
        }

        static void OnCompleted()
        {
            if (_result)
            {
                MessageBoxService.Show("Операция выполнена успешно");
            }
            else
            {
                MessageBoxService.Show("Ошибка при выполнении операции");
            }
        }
    }
}