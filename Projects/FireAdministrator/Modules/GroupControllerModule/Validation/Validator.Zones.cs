using System.Collections.Generic;
using System.Linq;
using RubezhAPI.GK;
using RubezhClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateZones()
		{
			ValidateZoneNoEquality();

			foreach (var zone in GKManager.Zones)
			{
				if (IsManyGK)
					ValidateDifferentGK(zone);
				//ValidateZoneHasNoDevices(zone);
				ValidateEmpty(zone);
				if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.HasFlag(ValidationErrorType.ZoneSensorQuantity))
				{
					ValidateZoneDetectorCount(zone);
					ValidateZoneFire1Fire2Count(zone);
				}
			}
		}

		void ValidateEmpty(GKZone zone)
		{
			if (zone.DataBaseParent == null)
			{
				Errors.Add(new ZoneValidationError(zone, "Пустые зависимости", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateZoneNoEquality()
		{
			var zoneNos = new HashSet<int>();
			foreach (var zone in GKManager.Zones)
			{
				if (!zoneNos.Add(zone.No))
					Errors.Add(new ZoneValidationError(zone, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateDifferentGK(GKZone zone)
		{
			if (zone.GkParents.Count > 0)
				Errors.Add(new ZoneValidationError(zone, "Зона содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
		}

		void ValidateZoneHasNoDevices(GKZone zone)
		{
			if (zone.Devices.Count == 0)
			{
				Errors.Add(new ZoneValidationError(zone, "К зоне не подключено ни одного устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateZoneDetectorCount(GKZone zone)
		{
			var fire1Count = zone.Devices.Count(x => x.Driver.AvailableStateBits.Contains(GKStateBit.Fire1));
			var fire2Count = zone.Devices.Count(x => x.Driver.AvailableStateBits.Contains(GKStateBit.Fire2));
			if (fire2Count == 0 && fire1Count < zone.Fire1Count)
			{
				Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 1", ValidationErrorLevel.CannotWrite));
				return;
			}
			if (fire2Count == 0 && fire1Count < zone.Fire2Count)
			{
				Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateZoneFire1Fire2Count(GKZone zone)
		{
			if (zone.Fire1Count >= zone.Fire2Count)
			{
				Errors.Add(new ZoneValidationError(zone, "Количество датчиков для сработки Пожар 1 должно быть меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}