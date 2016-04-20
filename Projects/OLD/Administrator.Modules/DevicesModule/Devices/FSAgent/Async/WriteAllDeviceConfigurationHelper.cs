using System;
using System.Collections.Generic;
using System.Text;
using FiresecAPI;
using FiresecClient;
using Infrastructure;
using Infrastructure.Common.Windows.Windows;

namespace DevicesModule.ViewModels
{
	public static class WriteAllDeviceConfigurationHelper
	{
		static OperationResult<bool> _operationResult;
		static List<string> Errors;

		public static void Run(bool showMessageBoxOnError = true)
		{
			Errors = new List<string>();
			foreach (var device in FiresecManager.Devices)
			{
				if (device.Driver.CanWriteDatabase)
				{
					var deviceName = device.PresentationAddressAndName + ". Запись конфигурации в устройство";
					ServiceFactory.ProgressService.Run(
						new Action
							(
							() =>
							{
								_operationResult = FiresecManager.DeviceWriteConfiguration(device, false);
								if (_operationResult.HasError)
								{
									Errors.Add(device.PresentationAddressAndName + " " + _operationResult.Error);
								}
							}
							),
						null, deviceName);
				}
			}
			if (showMessageBoxOnError)
			{
				if (Errors.Count > 0)
				{
					var stringBuilder = new StringBuilder();
					foreach (var error in Errors)
					{
						stringBuilder.AppendLine(error);
					}
					MessageBoxService.Show(stringBuilder.ToString());
				}
				else
				{
					MessageBoxService.Show("Операция завершилась успешно");
				}
			}
		}
	}
}