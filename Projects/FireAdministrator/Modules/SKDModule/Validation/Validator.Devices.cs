using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using System.Text.RegularExpressions;

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
						Errors.Add(new DeviceValidationError(device, "Считыватель не ведет ни к какой зоне", ValidationErrorLevel.Warning));
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
					var property = device.Properties.FirstOrDefault(x => x.Name == "Address");
					if (property == null || string.IsNullOrEmpty(property.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Отсутствует IP-адрес устройства", ValidationErrorLevel.CannotSave));
						continue;
					}

					const string pattern = @"^([01]\d\d?|[01]?[1-9]\d?|2[0-4]\d|25[0-3])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])\.([01]?\d\d?|2[0-4]\d|25[0-5])$";
					var address = property.StringValue;
					if (string.IsNullOrEmpty(address) || !Regex.IsMatch(address, pattern))
					{
						Errors.Add(new DeviceValidationError(device, "Не верно задан IP-адрес", ValidationErrorLevel.CannotSave));
						continue;
					}

					if (!ipAddresses.Add(property.StringValue))
					{
						Errors.Add(new DeviceValidationError(device, "Дублируется IP-адрес устройства", ValidationErrorLevel.CannotSave));
					}
				}
			}
		}
	}
}