using System;
using System.Linq;
using Controls.MessageBox;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class SynchronizeDeviceHelper
    {
        static Guid _deviceUID;
        static bool _isUsb;
        static bool _result;

        public static void Run(Guid deviceUID, bool isUsb)
        {
            _deviceUID = deviceUID;
            _isUsb = isUsb;

            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == _deviceUID);
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, device.PresentationAddressDriver + ". Установка времени");
        }

        static void OnPropgress()
        {
            _result = FiresecManager.SynchronizeDevice(_deviceUID, _isUsb);
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
