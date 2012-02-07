using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Controls.MessageBox;

namespace DevicesModule.ViewModels
{
    public static class DeviceGetSerialListHelper
    {
        static Device _device;
        static List<string> _serials;

        public static void Run(Device device)
        {
            _device = device;
            ServiceFactory.ProgressService.Run(OnPropgress, OnlCompleted, _device.PresentationAddressDriver + ". Получение списка устройств");
        }

        static void OnPropgress()
        {
            _serials = FiresecManager.DeviceGetSerialList(_device.UID);
        }

        static void OnlCompleted()
        {
            if (_serials == null)
            {
                MessageBoxService.Show("Ошибка при выполнении операции");
                return;
            }
            ServiceFactory.UserDialogs.ShowModalWindow(new BindMsViewModel(_device, _serials));
        }
    }
}