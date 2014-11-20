using System.Collections.Generic;
using System.Linq;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDirections()
		{
			ValidateDirectionNoEquality();

			foreach (var direction in GKManager.Directions)
			{
				if (IsManyGK())
					ValidateDifferentGK(direction);
			}
		}

		void ValidateDirectionNoEquality()
		{
			var directionNos = new HashSet<int>();
			foreach (var direction in GKManager.Directions)
			{
				if (!directionNos.Add(direction.No))
					Errors.Add(new DirectionValidationError(direction, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDifferentGK(GKDirection direction)
		{
			var devices = new List<GKDevice>();
			devices.AddRange(direction.InputDevices);
			devices.AddRange(direction.OutputDevices);
			foreach (var zone in direction.InputZones)
			{
				devices.AddRange(zone.Devices);
			}

			if (AreDevicesInSameGK(devices))
				Errors.Add(new DirectionValidationError(direction, "Направление содержит объекты устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}
	}
}