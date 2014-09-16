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
	}
}