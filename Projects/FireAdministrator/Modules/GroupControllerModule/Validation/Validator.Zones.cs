using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public static partial class Validator
	{
		static void ValidateZones()
		{
			ValidateZoneNoEquality();

			foreach (var zone in XManager.Zones)
			{
				if (IsManyGK())
					ValidateDifferentGK(zone);
				if (MustValidate("Количество подключенных к зоне датчиков"))
				{
					ValidateZoneDetectorCount(zone);
					ValidateZoneFire1Fire2Count(zone);
				}
			}
		}

		static void ValidateZoneNoEquality()
		{
			var zoneNos = new HashSet<int>();
			foreach (var zone in XManager.Zones)
			{
				if (!zoneNos.Add(zone.No))
					Errors.Add(new ZoneValidationError(zone, "Дублируется номер", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateDifferentGK(XZone zone)
		{
			if (AreDevicesInSameGK(zone.Devices))
				Errors.Add(new ZoneValidationError(zone, "Зона содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));

			if (AreDevicesInSameGK(zone.DevicesInLogic))
				Errors.Add(new ZoneValidationError(zone, "Зона участвует в логике устройств разных ГК", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateZoneDetectorCount(XZone zone)
		{
			var fire1Count = zone.Devices.Count(x => x.Driver.AvailableStateBits.Contains(XStateBit.Fire1));
			var fire2Count = zone.Devices.Count(x => x.Driver.AvailableStateBits.Contains(XStateBit.Fire2));
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

		static void ValidateZoneFire1Fire2Count(XZone zone)
		{
			if (zone.Fire1Count >= zone.Fire2Count)
			{
				Errors.Add(new ZoneValidationError(zone, "Количество датчиков для сработки Пожар 1 должно быть меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}