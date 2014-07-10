using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;
using System;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateDoors()
		{
			ValidateDoorsEquality();
			ValidateDoorsInDevicesUnique();

			foreach (var door in SKDManager.Doors)
			{
				if (string.IsNullOrEmpty(door.Name))
				{
					Errors.Add(new DoorValidationError(door, "Отсутствует название точки доступа", ValidationErrorLevel.CannotWrite));
				}

				var inDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == door.InDeviceUID);
				if (inDevice == null)
					Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует входное устройство", ValidationErrorLevel.CannotWrite));

				var outDevice = SKDManager.Devices.FirstOrDefault(x => x.UID == door.OutDeviceUID);
				if (outDevice == null)
					Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует выходное устройство", ValidationErrorLevel.CannotWrite));

				if (inDevice != null && inDevice.DriverType != SKDDriverType.Reader)
					Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует выходным устройством может быть только считыватель", ValidationErrorLevel.CannotWrite));

				if (inDevice != null && outDevice != null)
				{
					if (door.DoorType == DoorType.OneWay)
					{
						if (outDevice.DriverType != SKDDriverType.Button && outDevice.IntAddress != inDevice.IntAddress / 2)
							Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует неверно указано выходное устройство", ValidationErrorLevel.CannotWrite));
					}
					if (door.DoorType == DoorType.TwoWay)
					{
						if (outDevice.DriverType != SKDDriverType.Reader && outDevice.IntAddress != inDevice.IntAddress + 1)
							Errors.Add(new DoorValidationError(door, "У точки доступа отсутствует неверно указано выходное устройство", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}

		void ValidateDoorsEquality()
		{
			var doorNames = new HashSet<string>();
			foreach (var door in SKDManager.Doors)
			{
				if (!doorNames.Add(door.Name))
				{
					Errors.Add(new DoorValidationError(door, "Дублируется название точки доступа", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateDoorsInDevicesUnique()
		{
			var inDevicesUID = new HashSet<Guid>();
			foreach (var door in SKDManager.Doors)
			{
				if (door.InDeviceUID != Guid.Empty)
				{
					if (!inDevicesUID.Add(door.InDeviceUID))
					{
						Errors.Add(new DoorValidationError(door, "Одно и то же устройство привязано к разным точкам доступа", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}
	}
}