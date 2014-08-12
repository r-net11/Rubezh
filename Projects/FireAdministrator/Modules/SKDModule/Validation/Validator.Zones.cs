using System.Collections.Generic;
using FiresecAPI.SKD;
using Infrastructure.Common.Validation;

namespace SKDModule.Validation
{
	public partial class Validator
	{
		void ValidateZones()
		{
			ValidateZonesEquality();
			foreach (var zone in SKDManager.Zones)
			{
				if (string.IsNullOrEmpty(zone.Name))
				{
					Errors.Add(new ZoneValidationError(zone, "Отсутствует название зоны", ValidationErrorLevel.CannotSave));
				}
			}
		}

		void ValidateZonesEquality()
		{
			var zoneNames = new HashSet<string>();
			foreach (var zone in SKDManager.Zones)
			{
				if (!zoneNames.Add(zone.Name))
				{
					Errors.Add(new ZoneValidationError(zone, "Дублируется название зоны", ValidationErrorLevel.CannotSave));
				}
			}
		}
	}
}