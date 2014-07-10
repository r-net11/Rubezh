using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;

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
					Errors.Add(new DeviceValidationError(device, "Отсутствует название устройства", ValidationErrorLevel.CannotWrite));
				}
				if (device.DriverType == SKDDriverType.Controller)
				{
					if (device.Children.Count == 0)
					{
						Errors.Add(new DeviceValidationError(device, "Отсутствует подключенные устройства", ValidationErrorLevel.CannotWrite));
					}
					else
					{
						if (device.Children.Where(x => x.DriverType == SKDDriverType.Reader).Count() == 0)
							Errors.Add(new DeviceValidationError(device, "Отсутствуют считыватели", ValidationErrorLevel.CannotWrite));
						if (device.Children.Where(x => x.DriverType == SKDDriverType.Reader).Count() > 10)
							Errors.Add(new DeviceValidationError(device, "Количество считывателей не может быть больше 10", ValidationErrorLevel.CannotWrite));
					}
				}
				if (device.DriverType == SKDDriverType.Reader)
				{
					if (device.Zone == null)
					{
						Errors.Add(new DeviceValidationError(device, "Считыватель не ведет ни к какой зоне", ValidationErrorLevel.CannotWrite));
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

		void ValidateDevicesEquality()
		{
			foreach (var device in SKDManager.Devices)
			{
				var deviceNames = new HashSet<string>();
				foreach (var childDevice in device.Children)
				{
					if (!deviceNames.Add(childDevice.Name))
					{
						Errors.Add(new DeviceValidationError(childDevice, "Дублируется название устройства", ValidationErrorLevel.CannotWrite));
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
					var property = device.Properties.FirstOrDefault(x => x.Name == "Address");
					if (property == null || string.IsNullOrEmpty(property.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Отсутствует IP-адрес устройства", ValidationErrorLevel.CannotWrite));
					}
					else
					{
						if (!ipAddresses.Add(property.StringValue))
						{
							Errors.Add(new DeviceValidationError(device, "Дублируется IP-адрес устройства", ValidationErrorLevel.CannotWrite));
						}
					}
				}
			}
		}
	}
}