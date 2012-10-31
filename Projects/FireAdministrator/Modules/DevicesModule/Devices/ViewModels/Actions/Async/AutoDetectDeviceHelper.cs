using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;
using System;
using Common;

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
            ServiceFactory.ProgressService.Run(OnPropgress, OnCompleted, _deviceViewModel.Device.PresentationAddressAndDriver + ". Автопоиск устройств");
        }

        static void OnPropgress()
        {

            _operationResult = FiresecManager.AutoDetectDevice(_deviceViewModel.Device, _fastSearch);
        }

        static void OnCompleted()
        {
            if (_operationResult.HasError)
            {
                MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                Logger.Error("AutoDetectDeviceHelper.OnCompleted " + _operationResult.Error);
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
                //DeviceViewModels = _deviceViewModel.Source
            };

			if (DialogService.ShowModalWindow(autodetectionViewModel))
                RunAutodetection(false);
        }
    }
}