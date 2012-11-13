using System;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;

namespace GKModule.Validation
{
	public static partial class Validator
	{
		static void ValidateDevices()
		{
			ValidateAddressEquality();

			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (IsManyGK())
					ValidateDifferentGK(device);
				ValidateIPAddress(device);
				ValidateDeviceZone(device);
				ValidateDeviceLogic(device);
				ValidateGKNotEmptyChildren(device);
				ValidateKAUNotEmptyChildren(device);
				ValidatePumpAddresses(device);
			}
		}

		static void ValidateAddressEquality()
		{
			var deviceAddresses = new HashSet<string>();
			foreach (var device in XManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == XDriverType.System || device.Driver.DriverType == XDriverType.GK || !device.Driver.HasAddress || device.Driver.IsAutoCreate || device.Driver.IsGroupDevice)
					continue;

				if (!deviceAddresses.Add(device.DottedAddress))
					Errors.Add(new DeviceValidationError(device, "Дублиреутся адрес", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateIPAddress(XDevice device)
		{
			if (!XManager.IsValidIpAddress(device))
			{
				Errors.Add(new DeviceValidationError(device, "Не верно задан IP адрес", ValidationErrorLevel.CannotWrite));
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
				if (device.Zones.Count == 0)
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

		static void ValidateGKNotEmptyChildren(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.GK)
			{
				if (device.Children.Count <= 14)
					Errors.Add(new DeviceValidationError(device, "Устройство должно содержать подключенные устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateKAUNotEmptyChildren(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.KAU)
			{
				if (device.Children.Count <= 1)
					Errors.Add(new DeviceValidationError(device, "Устройство должно содержать подключенные устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidatePumpAddresses(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.Pump)
			{
				if (device.IntAddress < 1 || device.IntAddress > 8)
					Errors.Add(new DeviceValidationError(device, "Адрес устройства должен быть в диапазоне 1 - 8", ValidationErrorLevel.CannotWrite));
			}
		}
	}
}