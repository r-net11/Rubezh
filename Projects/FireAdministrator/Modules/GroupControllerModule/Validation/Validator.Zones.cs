﻿using System.Collections.Generic;
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

			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (IsManyGK())
					ValidateDifferentGK(zone);
				ValidateZoneDetectorCount(zone);
			}
		}

		static void ValidateZoneNoEquality()
		{
			var zoneNos = new HashSet<int>();
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				if (!zoneNos.Add(zone.No))
					Errors.Add(new ZoneValidationError(zone, "Дублиреутся адрес", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateDifferentGK(XZone zone)
		{
			if (AreDevicesInSameGK(zone.Devices))
				Errors.Add(new ZoneValidationError(zone, "Зона содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));

			if (AreDevicesInSameGK(zone.DevicesInLogic))
				Errors.Add(new ZoneValidationError(zone, "Зона учавствуе в логике устройств разных ГК", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateZoneDetectorCount(XZone zone)
		{
			var nonHandDetector = zone.Devices.Count(x => x.Driver.DriverType != XDriverType.HandDetector);
			if (zone.Fire1Count < nonHandDetector)
			{
				Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 1", ValidationErrorLevel.CannotWrite));
				return;
			}
			if (zone.Fire2Count < nonHandDetector)
			{
				Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}