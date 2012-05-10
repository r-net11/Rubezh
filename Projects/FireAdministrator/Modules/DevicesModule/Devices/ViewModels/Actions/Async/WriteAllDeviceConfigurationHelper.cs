using System;
using Infrastructure.Common.MessageBox;
using FiresecAPI;
using FiresecClient;
using Infrastructure;

namespace DevicesModule.ViewModels
{
    public static class WriteAllDeviceConfigurationHelper
    {
        static OperationResult<bool> _operationResult;

        public static void Run()
        {
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                bool hasError = false;
                if (device.Driver.CanWriteDatabase)
                {
                    var deviceName = device.PresentationAddressDriver + ". Запись конфигурации в устройство";
                    ServiceFactory.ProgressService.Run(
                        new Action
                            (
                            () =>
                            {
                                _operationResult = FiresecManager.DeviceWriteConfiguration(device.UID, false);
                                if (_operationResult.HasError)
                                {
                                    MessageBoxService.ShowDeviceError("Ошибка при выполнении операции", _operationResult.Error);
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