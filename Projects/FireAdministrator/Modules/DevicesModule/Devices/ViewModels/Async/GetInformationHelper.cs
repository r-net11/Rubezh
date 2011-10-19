using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class GetInformationHelper
    {
        static Device _device;
        static string _description;

        public static void Run(Device device)
        {
            _device = device;
            AsyncOperationHelper.Run(OnPropgress, OnlCompleted, _device.PresentationAddressDriver + ". Чтение информации об устройстве");
        }

        static void OnPropgress()
        {
            _description = FiresecManager.DeviceGetInformation(_device.UID);
        }

        static void OnlCompleted()
        {
            var deviceDescriptionViewModel = new DeviceDescriptionViewModel(_device.UID, _description);
            ServiceFactory.UserDialogs.ShowModalWindow(deviceDescriptionViewModel);
        }
    }
}
