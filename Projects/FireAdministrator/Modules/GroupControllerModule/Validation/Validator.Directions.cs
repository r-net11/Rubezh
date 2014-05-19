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

			foreach (var direction in XManager.Directions)
			{
				if (IsManyGK())
					ValidateDifferentGK(direction);
				if (ValidateEmptyDirection(direction))
				{
					if (MustValidate("В направлении отсутствуют входные устройства или зоны"))
						ValidateDirectionInputCount(direction);
					if (MustValidate("В направлении отсутствуют выходные устройства"))
					{
						ValidateDirectionOutputCount(direction);
					}

					ValidateEmptyZoneInDirection(direction);
					ValidateSameInputOutputDevices(direction);
					ValidateDirectionSelfLogic(direction);
				}
			}
		}

		void ValidateDirectionNoEquality()
		{
			var directionNos = new HashSet<int>();
			foreach (var direction in XManager.Directions)
			{
				if (!directionNos.Add(direction.No))
					Errors.Add(new DirectionValidationError(direction, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDifferentGK(XDirection direction)
		{
			var devices = new List<XDevice>();
			devices.AddRange(direction.InputDevices);
			devices.AddRange(direction.OutputDevices);
			foreach (var zone in direction.InputZones)
			{
				devices.AddRange(zone.Devices);
			}

			if (AreDevicesInSameGK(devices))
				Errors.Add(new DirectionValidationError(direction, "Направление содержит объекты устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		void ValidateDirectionInputCount(XDirection direction)
		{
			if (direction.InputDevices.Count + direction.InputZones.Count == 0)
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные устройства или зоны", ValidationErrorLevel.CannotWrite));
		}

		void ValidateDirectionOutputCount(XDirection direction)
		{
			if (direction.OutputDevices.Count == 0)
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют выходные устройства", ValidationErrorLevel.CannotWrite));
		}

		bool ValidateEmptyDirection(XDirection direction)
		{
			var count = direction.InputZones.Count + direction.InputDevices.Count + direction.OutputDevices.Count;
			if (count == 0)
			{
				Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные или выходные объекты", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}

		void ValidateEmptyZoneInDirection(XDirection direction)
		{
			foreach (var zone in direction.InputZones)
			{
				if (zone.Devices.Count == 0)
				{
					Errors.Add(new DirectionValidationError(direction, "В направление входит пустая зона " + zone.PresentationName, ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateSameInputOutputDevices(XDirection direction)
		{
			foreach (var device in direction.OutputDevices)
			{
				if (direction.InputDevices.Any(x => x.BaseUID == device.BaseUID))
				{
					Errors.Add(new DirectionValidationError(direction, "Устройство " + device.PresentationName + " участвует во входных и выходных зависимостях направления", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateDirectionSelfLogic(XDirection direction)
		{
			if (direction.ClauseInputDirections.Contains(direction))
				Errors.Add(new DirectionValidationError(direction, "Направление зависит от самого себя", ValidationErrorLevel.CannotWrite));
		}
	}
}