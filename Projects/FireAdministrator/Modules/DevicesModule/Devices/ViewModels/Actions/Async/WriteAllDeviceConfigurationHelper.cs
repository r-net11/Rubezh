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

		public static void Run(bool showMessageBoxOnError = true)
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
									if (showMessageBoxOnError)
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
			if (showMessageBoxOnError)
				MessageBoxService.Show("Операция завершилась успешно");
		}
	}
}