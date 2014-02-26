using System;
using System.Collections.Generic;
using FiresecAPI;
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
					Errors.Add(new ZoneValidationError(zone, "Отсутствует название зоны", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateZonesEquality()
		{
			foreach (var zone in SKDManager.Zones)
			{
				var zoneNames = new HashSet<string>();
				foreach (var childZone in zone.Children)
				{
					if (!zoneNames.Add(childZone.Name))
					{
						Errors.Add(new ZoneValidationError(childZone, "Дублируется название зоны", ValidationErrorLevel.CannotWrite));
					}
				}
			}
		}
	}
}