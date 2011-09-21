using FiresecClient;
using Infrastructure;
using FiresecAPI.Models;

namespace DevicesModule.ViewModels
{
    public static class DeviceJournalHelper
    {
        static Device _device;
        static string _journal;

        public static void Run(Device device)
        {
            _device = device;
            AsyncOperationHelper.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Чтение журнала");
        }

        static void OnPropgress()
        {
            _journal = FiresecManager.ReadDeviceJournal(_device.UID);
        }

        static void OnCompleted()
        {
            var deviceJournalViewModel = new DeviceJournalViewModel(_journal);
            ServiceFactory.UserDialogs.ShowModalWindow(deviceJournalViewModel);
        }
    }
}
