using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class AutoSearchHelper
    {
        static DeviceViewModel _deviceViewModel;
        static DeviceConfiguration _autodetectedDeviceConfiguration;
        static bool _fastSearch;

        public static void Run(DeviceViewModel deviceViewModel)
        {
            _deviceViewModel = deviceViewModel;
            RunAutodetection(true);
        }

        static void RunAutodetection(bool fastSearch)
        {
            _fastSearch = fastSearch;
            AsyncOperationHelper.Run(OnPropgress, OnlCompleted, _deviceViewModel.Device.PresentationAddressDriver + ". Автопоиск устройств");
        }

        static void OnPropgress()
        {
            _autodetectedDeviceConfiguration = FiresecManager.AutoDetectDevice(_deviceViewModel.Device.UID, _fastSearch);
        }

        static void OnlCompleted()
        {
            if (_autodetectedDeviceConfiguration == null)
            {
                DialogBox.DialogBox.Show("Операция прервана");
                return;
            }

            _autodetectedDeviceConfiguration.Update();

            foreach (var device in _autodetectedDeviceConfiguration.Devices)
            {
                var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID.ToString().ToUpper() == device.DriverUID.ToString().ToUpper());
                device.Driver = driver;
            }

            var autodetectionViewModel = new AutoSearchViewModel();
            autodetectionViewModel.DeviceViewModels = _deviceViewModel.Source;
            autodetectionViewModel.Initialize(_autodetectedDeviceConfiguration);
            bool result = ServiceFactory.UserDialogs.ShowModalWindow(autodetectionViewModel);
            if (result)
            {
                RunAutodetection(false);
            }
        }
    }
}