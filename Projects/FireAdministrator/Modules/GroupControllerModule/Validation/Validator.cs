using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public static class Validator
	{
		private static List<IValidationError> _errors { get; set; }

		public static IEnumerable<IValidationError> Validate()
		{
			_errors = new List<IValidationError>();
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
			return _errors;
		}

		static void ValidateDevices()
		{

		}

		static void ValidateZones()
		{
			foreach (var zone in XManager.DeviceConfiguration.Zones)
			{
				ValidateZoneDetectorCount(zone);
			}
		}

		static void ValidateZoneDetectorCount(XZone zone)
		{
			if (zone.Devices == null || zone.Devices.Any(x => x.Driver.DriverType == XDriverType.HandDetector))
				return;
			if (zone.Fire1Count > zone.Devices.Count)
				_errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки", ValidationErrorLevel.Warning));
		}

		static void ValidateDirections()
		{

		}
	}
}