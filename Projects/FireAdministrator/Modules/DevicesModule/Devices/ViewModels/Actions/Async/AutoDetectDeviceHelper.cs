using System.Linq;
using Infrastructure.Common.MessageBox;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class AutoDetectDeviceHelper
    {
        static DeviceViewModel _deviceViewModel;
        static OperationResult<DeviceConfiguration> _operationResult;
        static bool _fastSearch;

        public static void Run(DeviceViewModel deviceViewModel)
        {
            _deviceViewModel = deviceViewModel;
            RunAutodetection(true);
        }

        static void RunAutodetection(bool fastSearch)
        {
            _fastSearch = fastSearch;
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _deviceViewModel.Device.PresentationAddressDriver + ". Автопоиск устройств");
        }

        static void OnPropgress()
        {
            _operationResult = FiresecManager.AutoDetectDevice(_deviceViewModel.Device.UID, _fastSearch);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
                return;
            }

            var deviceConfiguration = _operationResult.Result;
            deviceConfiguration.Update();

            foreach (var device in deviceConfiguration.Devices)
            {
                var driver = FiresecManager.Drivers.FirstOrDefault(x => x.UID.ToString().ToUpper() == device.DriverUID.ToString().ToUpper());
                device.Driver = driver;
            }

            var autodetectionViewModel = new AutoSearchViewModel(deviceConfiguration)
            {
                DeviceViewModels = _deviceViewModel.Source
            };

            if (ServiceFactory.UserDialogs.ShowModalWindow(autodetectionViewModel))
                RunAutodetection(false);
        }
    }
}