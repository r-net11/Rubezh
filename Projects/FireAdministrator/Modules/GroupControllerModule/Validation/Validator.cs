using System.Collections.Generic;
using System.Linq;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;
using System.Net;
using System;

namespace GKModule.Validation
{
	public static class Validator
	{
		static List<IValidationError> Errors { get; set; }

		public static IEnumerable<IValidationError> Validate()
		{
			Errors = new List<IValidationError>();
			ValidateDevices();
			ValidateZones();
			ValidateDirections();
			return Errors;
		}

		static void ValidateDevices()
		{
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				ValidateIPAddress(device);
			}
		}

		static void ValidateIPAddress(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.GK)
			{
				var address = device.GetGKIpAddress();
				if (!CheckIpAddress(address))
				{
					Errors.Add(new DeviceValidationError(device, "Не задан IP адрес", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		static bool CheckIpAddress(string ipAddress)
		{
			if (String.IsNullOrEmpty(ipAddress))
				return false;
			IPAddress address;
			return IPAddress.TryParse(ipAddress, out address);
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
				Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateDirections()
		{
			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
			}
		}
	}
}