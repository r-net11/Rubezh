using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using System.Text.RegularExpressions;
using System.Net;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateDevices()
		{
			ValidateDevicesEquality();
			ValidateIPAddresses();

			foreach (var device in SKDManager.Devices)
			{
				if (string.IsNullOrEmpty(device.Name))
				{
					Errors.Add(new DeviceValidationError(device, "Отсутствует название устройства", ValidationErrorLevel.CannotSave));
				}
				if (device.DriverType == SKDDriverType.Reader)
				{
					if (device.Zone == null)
					{
						Errors.Add(new DeviceValidationError(device, "Считыватель не ведет в зону", ValidationErrorLevel.Warning));
					}
					if (device.Door == null)
					{
						Errors.Add(new DeviceValidationError(device, "Считыватель не участвует в точке доступа", ValidationErrorLevel.Warning));
					}
				}

				ValidateLockIntervalsConfiguration(device);
			}
		}

		void ValidateLockIntervalsConfiguration(SKDDevice device)
		{
			if (device.DriverType == SKDDriverType.Lock)
			{
				foreach (var doorDayInterval in device.SKDDoorConfiguration.DoorDayIntervalsCollection.DoorDayIntervals)
				{
					var currentMinutes = 0;
					foreach (var doorDayIntervalPart in doorDayInterval.DoorDayIntervalParts)
					{
						var startTime = doorDayIntervalPart.StartHour * 60 + doorDayIntervalPart.StartMinute;
						var endTime = doorDayIntervalPart.EndHour * 60 + doorDayIntervalPart.EndMinute;

						if (startTime > 0 && endTime > 0)
						{
							var dayIndex = device.SKDDoorConfiguration.DoorDayIntervalsCollection.DoorDayIntervals.IndexOf(doorDayInterval);
							var intervalPartIndex = doorDayInterval.DoorDayIntervalParts.IndexOf(doorDayIntervalPart) + 1;
							var comment = " (" + ViewModels.WeeklyIntervalPartViewModel.IntToWeekDay(dayIndex + 1) + "/" + intervalPartIndex + ")";

							if (endTime < startTime)
							{
								Errors.Add(new DeviceValidationError(device, "Начало интервала меньше конца интервала в настройке замка" + comment, ValidationErrorLevel.CannotWrite));
								break;
							}

							if (startTime < currentMinutes)
							{
								Errors.Add(new DeviceValidationError(device, "Последовательность интервалов в настройке замка не должна быть пересекающейся" + comment, ValidationErrorLevel.CannotWrite));
								break;
							}
							currentMinutes = endTime;
						}
					}
				}
			}
		}

		void ValidateDevicesEquality()
		{
			foreach (var device in SKDManager.Devices)
			{
				var deviceNames = new HashSet<string>();
				foreach (var childDevice in device.Children)
				{
					if (!deviceNames.Add(childDevice.Name))
					{
						Errors.Add(new DeviceValidationError(childDevice, "Дублируется название устройства", ValidationErrorLevel.CannotSave));
					}
				}
			}
		}

		void ValidateIPAddresses()
		{
			var ipAddresses = new HashSet<string>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.Driver.IsController)
				{
					var addressProperty = device.Properties.FirstOrDefault(x => x.Name == "Address");
					if (addressProperty == null || !SKDManager.ValidateIPAddress(addressProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не верно задан IP-адрес", ValidationErrorLevel.CannotSave));
						continue;
					}

					if (!ipAddresses.Add(addressProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Дублируется IP-адрес устройства", ValidationErrorLevel.CannotSave));
					}

					var maskProperty = device.Properties.FirstOrDefault(x => x.Name == "Mask");
					if (maskProperty == null || !SKDManager.ValidateIPAddress(maskProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не верно задана маска подсети", ValidationErrorLevel.CannotSave));
						continue;
					}

					var gatewayProperty = device.Properties.FirstOrDefault(x => x.Name == "Gateway");
					if (gatewayProperty == null || !SKDManager.ValidateIPAddress(gatewayProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не верно задан шлюз по умолчанию", ValidationErrorLevel.CannotSave));
						continue;
					}

					var loginProperty = device.Properties.FirstOrDefault(x => x.Name == "Login");
					if (loginProperty == null || string.IsNullOrEmpty(loginProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не задан логин", ValidationErrorLevel.CannotSave));
						continue;
					}

					var passwordProperty = device.Properties.FirstOrDefault(x => x.Name == "Password");
					if (passwordProperty == null || string.IsNullOrEmpty(passwordProperty.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Не задан пароль", ValidationErrorLevel.CannotSave));
						continue;
					}

					var portProperty = device.Properties.FirstOrDefault(x => x.Name == "Port");
					if (portProperty == null || portProperty.Value <= 0)
					{
						Errors.Add(new DeviceValidationError(device, "Не задан порт", ValidationErrorLevel.CannotSave));
						continue;
					}
				}
			}
		}
	}
}