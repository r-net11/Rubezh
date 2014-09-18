using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDoors()
		{
			ValidateDoorNoEquality();

			foreach (var door in XManager.DeviceConfiguration.Doors)
			{
				ValidateDoorHasNoDevices(door);
				ValidateDoorHasWrongDevices(door);
			}
		}

		void ValidateDoorNoEquality()
		{
			var doorNos = new HashSet<int>();
			foreach (var door in XManager.DeviceConfiguration.Doors)
			{
				if (!doorNos.Add(door.No))
					Errors.Add(new DoorValidationError(door, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDoorHasNoDevices(XDoor door)
		{
			if (door.EnterDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на вход", ValidationErrorLevel.CannotWrite));
			}
			if (door.ExitDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на выход", ValidationErrorLevel.CannotWrite));
			}
			if (door.LockDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключен замок", ValidationErrorLevel.CannotWrite));
			}
			if (door.LockControlDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключен датчик контроля двери", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDoorHasWrongDevices(XDoor door)
		{
			if (door.EnterDevice != null && door.EnterDevice.DriverType != XDriverType.RSR2_CodeReader)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на вход", ValidationErrorLevel.CannotWrite));
			}

			if (door.ExitDevice != null)
			{
				if (door.DoorType == XDoorType.OneWay && door.ExitDevice.DriverType != XDriverType.AM_1)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
				if (door.DoorType == XDoorType.TwoWay && door.ExitDevice.DriverType != XDriverType.RSR2_CodeReader)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
			}

			if (door.LockDevice != null && door.LockDevice.DriverType != XDriverType.RSR2_RM_1 && door.LockDevice.DriverType != XDriverType.RSR2_MVK8)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на замок", ValidationErrorLevel.CannotWrite));
			}

			if (door.LockControlDevice != null && door.LockControlDevice.DriverType != XDriverType.RSR2_AM_1)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на датчик контроля двери", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}