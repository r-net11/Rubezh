using Controls.MessageBox;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class DeviceGetMDS5DataHelper
    {
        static Device _device;
        static string _mDS5Data;

        public static void Run(Device device)
        {
            _device = device;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Получение данных с модуля доставки сообщений");
        }

        static void OnPropgress()
        {
            _mDS5Data = FiresecManager.DeviceGetMDS5Data(_device.UID);
        }

        static void OnCompleted()
        {
            if (_mDS5Data == null)
            {
                MessageBoxService.Show("Ошибка при выполнении операции");
                return;
            }
            MessageBoxService.Show(_mDS5Data);
        }
    }
}
