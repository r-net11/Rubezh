using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class DeviceGetInformationHelper
    {
        static Device _device;
        static bool _isUsb;
        static string _description;

        public static void Run(Device device, bool isUsb)
        {
            _device = device;
            _isUsb = isUsb;
            AsyncOperationHelper.Run(OnPropgress, OnlCompleted, _device.PresentationAddressDriver + ". Чтение информации об устройстве");
        }

        static void OnPropgress()
        {
            _description = FiresecManager.DeviceGetInformation(_device.UID, _isUsb);
        }

        static void OnlCompleted()
        {
            if (_description == null)
            {
                DialogBox.DialogBox.Show("Ошибка при выполнении операции");
                return;
            }
            ServiceFactory.UserDialogs.ShowModalWindow(new DeviceDescriptionViewModel(_device.UID, _description));
        }
    }
}