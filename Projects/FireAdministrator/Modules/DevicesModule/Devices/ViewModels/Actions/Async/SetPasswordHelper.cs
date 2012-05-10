using System;
using System.Linq;
using Infrastructure.Common.MessageBox;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class SetPasswordHelper
    {
        static Guid _deviceUID;
        static bool _isUsb;
        static DevicePasswordType _devicePasswordType;
        static string _password;
        static OperationResult<bool> _operationResult;

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
            _operationResult = FiresecManager.SetPassword(_deviceUID, _isUsb, _devicePasswordType, _password);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}