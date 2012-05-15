using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.MessageBox;

namespace DevicesModule.ViewModels
{
    public static class ReadDeviceJournalHelper
    {
        static Device _device;
        static bool _isUsb;
        static OperationResult<string> _operationResult;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;

            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _device.PresentationAddressDriver + ". Чтение журнала");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.ReadDeviceJournal(_device.UID, _isUsb);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }
            ServiceFactory.UserDialogs.ShowModalWindow(new DeviceJournalViewModel(_operationResult.Result));
        }
    }
}