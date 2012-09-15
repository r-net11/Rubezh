using System;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows;

namespace DevicesModule.ViewModels
{
    public static class WriteAllDeviceConfigurationHelper
    {
        static OperationResult<bool> _operationResult;

        public static void Run()
        {
			foreach (var device in FiresecManager.Devices)
            {
                bool hasError = false;
                if (device.Driver.CanWriteDatabase)
                {
                    var deviceName = device.PresentationAddressAndDriver + ". Запись конфигурации в устройство";
                    ServiceFactory.ProgressService.Run(
                        new Action
                            (
                            () =>
                            {
                                _operationResult = FiresecManager.DeviceWriteConfiguration(device, false);
                                if (_operationResult.HasError)
                                {
									MessageBoxService.ShowError(_operationResult.Error, "Ошибка при выполнении операции");
                                    hasError = true;
                                    return;
                                }
                            }
                            ),
                        null, deviceName);
                }
                if (hasError)
                    return;
            }
            MessageBoxService.Show("Операция завершилась успешно");
        }
    }
}