using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;
using System;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDoors()
		{
			ValidateDoorNoEquality();
			ValidateDoorSameDevices();

			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				ValidateDoorHasNoDevices(door);
				ValidateDoorHasWrongDevices(door);
				ValidateLockLogic(door);
			}
		}

		void ValidateDoorNoEquality()
		{
			var doorNos = new HashSet<int>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				if (!doorNos.Add(door.No))
					Errors.Add(new DoorValidationError(door, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDoorSameDevices()
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var door in GKManager.DeviceConfiguration.Doors)
			{
				if (door.EnterDevice != null)
				{
					if(!deviceUIDs.Add(door.EnterDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.EnterDevice.PresentationName + " уже участвует в другой точке доступа", ValidationErrorLevel.CannotWrite));
				}
				if (door.ExitDevice != null)
				{
					if (!deviceUIDs.Add(door.ExitDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.ExitDevice.PresentationName + " уже участвует в другой точке доступа", ValidationErrorLevel.CannotWrite));
				}
				if (door.LockDevice != null)
				{
					if (!deviceUIDs.Add(door.LockDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.LockDevice.PresentationName + " уже участвует в другой точке доступа", ValidationErrorLevel.CannotWrite));
				}
				if (door.LockControlDevice != null)
				{
					if (!deviceUIDs.Add(door.LockControlDevice.UID))
						Errors.Add(new DoorValidationError(door, "Устройство " + door.LockControlDevice.PresentationName + " уже участвует в другой точке доступа", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateDoorHasNoDevices(GKDoor door)
		{
			if (door.EnterDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на вход", ValidationErrorLevel.CannotWrite));
			}
			if (door.DoorType == GKDoorType.TwoWay && door.ExitDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено устройство на выход", ValidationErrorLevel.CannotWrite));
			}
			if (door.LockDevice == null)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключен замок", ValidationErrorLevel.CannotWrite));
			}
			//if (door.LockControlDevice == null)
			//{
			//	Errors.Add(new DoorValidationError(door, "К точке доступа не подключен датчик контроля двери", ValidationErrorLevel.CannotWrite));
			//}
		}

		void ValidateDoorHasWrongDevices(GKDoor door)
		{
			if (door.EnterDevice != null && door.EnterDevice.DriverType != GKDriverType.RSR2_CodeReader && door.EnterDevice.DriverType != GKDriverType.RSR2_CardReader)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на вход", ValidationErrorLevel.CannotWrite));
			}

			if (door.ExitDevice != null)
			{
				if (door.DoorType == GKDoorType.OneWay && door.ExitDevice.DriverType != GKDriverType.RSR2_AM_1)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
				if (door.DoorType == GKDoorType.TwoWay && door.ExitDevice.DriverType != GKDriverType.RSR2_CodeReader && door.ExitDevice.DriverType != GKDriverType.RSR2_CardReader)
				{
					Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на выход", ValidationErrorLevel.CannotWrite));
				}
			}

			if (door.LockDevice != null && door.LockDevice.DriverType != GKDriverType.RSR2_RM_1 && door.LockDevice.DriverType != GKDriverType.RSR2_MVK8)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на замок", ValidationErrorLevel.CannotWrite));
			}

			if (door.LockControlDevice != null && door.LockControlDevice.DriverType != GKDriverType.RSR2_AM_1)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа подключено неверное устройство на датчик контроля двери", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateLockLogic(GKDoor door)
		{
			if (door.LockDevice != null)
			{
				if (door.LockDevice.Logic.GetObjects().Count > 0)
				{
					Errors.Add(new DoorValidationError(door, "Устройство Замок не должно иметь настроенную логику срабатывания", ValidationErrorLevel.CannotWrite));
				}
			}
		}
	}
}