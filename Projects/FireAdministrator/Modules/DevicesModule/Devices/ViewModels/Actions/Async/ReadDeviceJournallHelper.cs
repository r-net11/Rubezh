using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class ReadDeviceJournalHelper
    {
        static Device _device;
        static bool _isUsb;
        static string _journal;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;

            AsyncOperationHelper.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Чтение журнала");
        }

        static void OnPropgress()
        {
            _journal = FiresecManager.ReadDeviceJournal(_device.UID, _isUsb);
        }

        static void OnCompleted()
        {
            if (_journal == null)
            {
                DialogBox.DialogBox.Show("Ошибка при выполнении операции");
                return;
            }
            ServiceFactory.UserDialogs.ShowModalWindow(new DeviceJournalViewModel(_journal));
        }
    }
}