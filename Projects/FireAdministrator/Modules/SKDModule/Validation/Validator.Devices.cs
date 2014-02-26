using System;
using System.Linq;
using System.Collections.Generic;
using FiresecAPI;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateDevices()
		{
			ValidateDevicesEquality();
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
						if (device.Children.Where(x => x.DriverType == SKDDriverType.Reader).Count() > 2)
							Errors.Add(new DeviceValidationError(device, "Количество считывателей не быть больше двух", ValidationErrorLevel.CannotWrite));
					}
				}
				if (device.DriverType == SKDDriverType.Reader)
				{
					if (device.OuterZone == null)
					{
						Errors.Add(new DeviceValidationError(device, "Считыватель не ведет ни к какой зоне", ValidationErrorLevel.CannotWrite));
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
	}
}