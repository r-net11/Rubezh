using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

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
                if (IsManyGK())
                    ValidateDifferentGK(device);
                ValidateIPAddress(device);
                ValidateDeviceZone(device);
                ValidateDeviceLogic(device);
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

        static void ValidateDifferentGK(XDevice device)
        {
            foreach (var clause in device.DeviceLogic.Clauses)
            {
                foreach (var clauseDevice in clause.Devices)
                {
                    if (device.GKParent != null && clauseDevice.GKParent != null && device.GKParent != clauseDevice.GKParent)
                        Errors.Add(new DeviceValidationError(device, "Логика сработки содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
                }
            }
        }

        static void ValidateDeviceZone(XDevice device)
        {
            if (device.Driver.HasZone)
            {
                if(device.Zones.Count == 0)
                    Errors.Add(new DeviceValidationError(device, "Устройство не подключено к зоне", ValidationErrorLevel.CannotWrite));
            }
        }

        static void ValidateDeviceLogic(XDevice device)
        {
            if (device.Driver.HasLogic)
            {
                if (device.DeviceLogic.Clauses.Count == 0)
                    Errors.Add(new DeviceValidationError(device, "Отсутствует логика срабатывания исполнительного устройства", ValidationErrorLevel.CannotWrite));
            }
        }

        static void ValidateZones()
        {
            foreach (var zone in XManager.DeviceConfiguration.Zones)
            {
                if (IsManyGK())
                    ValidateDifferentGK(zone);
                ValidateZoneDetectorCount(zone);
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
			if (zone.Devices.Any(x => x.Driver.DriverType == XDriverType.HandDetector))
				return;
            if (zone.Fire1Count > zone.Devices.Count)
            {
                Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 1", ValidationErrorLevel.CannotWrite));
            }
            //if (zone.Fire2Count > zone.Devices.Count)
            //    Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне датчиков меньше количества датчиков для сработки Пожар 2", ValidationErrorLevel.CannotWrite));
		}

		static void ValidateDirections()
		{
			foreach (var direction in XManager.DeviceConfiguration.Directions)
			{
                if (IsManyGK())
                    ValidateDifferentGK(direction);
                ValidateDirectionInputCount(direction);
                ValidateDirectionOutputCount(direction);
			}
		}

        static void ValidateDifferentGK(XDirection direction)
        {
            if (AreDevicesInSameGK(direction.InputDevices))
                Errors.Add(new DirectionValidationError(direction, "Направление содержит входные устройства разных ГК", ValidationErrorLevel.CannotWrite));

            if (AreDevicesInSameGK(direction.OutputDevices))
                Errors.Add(new DirectionValidationError(direction, "Направление содержит выходные устройств разных ГК", ValidationErrorLevel.CannotWrite));

            var devicesInZones = new List<XDevice>();
            foreach (var zone in direction.InputZones)
            {
                devicesInZones.AddRange(zone.Devices);
            }

            if (AreDevicesInSameGK(devicesInZones))
                Errors.Add(new DirectionValidationError(direction, "Направление содержит выходные зоны разных ГК", ValidationErrorLevel.CannotWrite));
        }

        static void ValidateDirectionInputCount(XDirection direction)
        {
            if(direction.InputDevices.Count + direction.InputZones.Count == 0)
                Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют входные устройства или зоны", ValidationErrorLevel.CannotWrite));
        }

        static void ValidateDirectionOutputCount(XDirection direction)
        {
            if (direction.OutputDevices.Count == 0)
                Errors.Add(new DirectionValidationError(direction, "В направлении отсутствуют выходные устройства", ValidationErrorLevel.CannotWrite));
        }

        static bool IsManyGK()
        {
            return XManager.DeviceConfiguration.Devices.Where(x=>x.Driver.DriverType == XDriverType.GK).Count() > 1;
        }

        static bool AreDevicesInSameGK(List<XDevice> devices)
        {
            var gkDevices = new HashSet<XDevice>();
            foreach (var device in devices)
            {
                if (device.GKParent != null)
                    gkDevices.Add(device.GKParent);
            }
            return (gkDevices.Count > 0);
        }
	}
}