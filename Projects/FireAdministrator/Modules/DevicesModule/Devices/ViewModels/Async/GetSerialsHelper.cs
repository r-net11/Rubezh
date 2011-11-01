using System.Collections.Generic;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class GetSerialsHelper
    {
        static Device _device;
        static List<string> _serials;

        public static void Run(Device device)
        {
            _device = device;
            AsyncOperationHelper.Run(OnPropgress, OnlCompleted, _device.PresentationAddressDriver + ". Получение списка устройств");
        }

        static void OnPropgress()
        {
            _serials = FiresecManager.DeviceGetSerialList(_device.UID);
        }

        static void OnlCompleted()
        {
            ServiceFactory.UserDialogs.ShowModalWindow(new BindMsViewModel(_device, _serials));
        }
    }
}