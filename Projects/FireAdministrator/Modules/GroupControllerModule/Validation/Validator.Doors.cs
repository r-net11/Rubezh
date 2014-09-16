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
			if (false)
			{
				Errors.Add(new DoorValidationError(door, "К точке доступа не подключено ни одного устройства", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}