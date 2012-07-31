using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

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
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressAndDriver + ". Установка пароля");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.SetPassword(_deviceUID, _isUsb, _devicePasswordType, _password);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
				MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                return;
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}