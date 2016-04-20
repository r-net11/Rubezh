using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common.Windows.Validation;

namespace DevicesModule.Validation
{
	partial class Validator
	{
		void ValidateDirections()
		{
			foreach (var direction in _firesecConfiguration.DeviceConfiguration.Directions)
			{
				if (ValidateDirectionZonesContent(direction))
				{ }
			}
		}

		bool ValidateDirectionZonesContent(Direction direction)
		{
			if (direction.ZoneUIDs.IsNotNullOrEmpty() == false)
			{
				Errors.Add(new DirectionValidationError(direction, "В направлении тушения нет ни одной зоны", ValidationErrorLevel.CannotWrite));
				return false;
			}
			return true;
		}
	}
}