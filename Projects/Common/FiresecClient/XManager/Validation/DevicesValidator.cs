using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;
using FiresecClient.Validation;

namespace FiresecClient.XModelsValidator
{
	public static class DevicesValidator
	{
		public static List<ZoneError> ZoneErrors { get; set; }

		public static void Validate()
		{
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
		}

		static void ValidateDevices()
		{

		}

		static void ValidateZones()
		{
			ZoneErrors = new List<ZoneError>();
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				ValidateZoneDetectorCount(zone);
			}
		}

		static void ValidateZoneDetectorCount(XZone zone)
		{
			if (zone.Devices.Any(x => x.Driver.DriverType == XDriverType.HandDetector))
				return;
			if (zone.DetectorCount > zone.Devices.Count)
				ZoneErrors.Add(new ZoneError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки", ErrorLevel.Warning));
		}

		static void ValidateDirections()
		{

		}
	}
}