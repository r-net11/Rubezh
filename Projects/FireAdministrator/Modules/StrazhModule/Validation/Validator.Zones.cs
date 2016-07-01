using System.Collections.Generic;
using Localization.Strazh.Errors;
using StrazhAPI.SKD;
using Infrastructure.Common.Validation;

namespace StrazhModule.Validation
{
	public partial class Validator
	{
		void ValidateZones()
		{
			ValidateZoneNoEquality();
			ValidateZonesEquality();

			foreach (var zone in SKDManager.Zones)
			{
				if (string.IsNullOrEmpty(zone.Name))
				{
					Errors.Add(new ZoneValidationError(zone, CommonErrors.ValidateZones_EmptyNameError, ValidationErrorLevel.CannotSave));
				}
			}
		}

		void ValidateZoneNoEquality()
		{
			var zoneNos = new HashSet<int>();
			foreach (var zone in SKDManager.Zones)
			{
				if (!zoneNos.Add(zone.No))
					Errors.Add(new ZoneValidationError(zone, CommonErrors.ValidateZones_DublicateError, ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateZonesEquality()
		{
			var zoneNames = new HashSet<string>();
			foreach (var zone in SKDManager.Zones)
			{
				if (!zoneNames.Add(zone.Name))
				{
					Errors.Add(new ZoneValidationError(zone, CommonErrors.ValidateZones_DublicateError, ValidationErrorLevel.CannotSave));
				}
			}
		}
	}
}