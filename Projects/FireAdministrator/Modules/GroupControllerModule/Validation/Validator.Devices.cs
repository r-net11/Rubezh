using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FiresecClient;
using Infrastructure.Common.Validation;
using XFiresecAPI;
using Common.GK;
using Infrastructure.Common;

namespace GKModule.Validation
{
	public static partial class Validator
	{
		static void ValidateDevices()
		{
			ValidateAddressEquality();

			foreach (var device in XManager.Devices)
			{
				if (device.IsNotUsed)
					continue;
				if (IsManyGK())
					ValidateDifferentGK(device);
				ValidateIPAddress(device);
				if (MustValidate("Устройство не подключено к зоне"))
					ValidateDeviceZone(device);
				if (MustValidate("Отсутствует логика срабатывания исполнительного устройства"))
					ValidateDeviceLogic(device);
				if (MustValidate("Устройство должно содержать подключенные устройства"))
				{
					ValidateGKNotEmptyChildren(device);
					ValidateKAUNotEmptyChildren(device);
				}
				ValidatePumpAddresses(device);
				ValidateParametersMinMax(device);
				ValidateNotUsedLogic(device);
				ValidateAddressEquality(device);
				ValidateRSR2AddressFollowing(device);
			}
		}

		static bool MustValidate(string errorName)
		{
			return GlobalSettingsHelper.GlobalSettings.IgnoredErrors == null || !GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(errorName);
		}

		static void ValidateAddressEquality()
		{
			var deviceAddresses = new HashSet<string>();
			foreach (var device in XManager.Devices)
			{
				if (device.Driver.DriverType == XDriverType.System || device.Driver.DriverType == XDriverType.GK || !device.Driver.HasAddress || device.Driver.IsAutoCreate || device.Driver.IsGroupDevice)
					continue;

				if (!deviceAddresses.Add(device.DottedAddress))
				{
					var x = device.DottedAddress;
					Errors.Add(new DeviceValidationError(device, "Дублируется адрес", ValidationErrorLevel.CannotWrite));
				}
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
					Errors.Add(new DeviceValidationError(device, "Устройство не подключено к зоне", ValidationErrorLevel.Warning));
			}
		}

		static void ValidateDeviceLogic(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.GKLine || device.Driver.DriverType == XDriverType.GKRele)
				return;

			if (device.Driver.HasLogic && !device.Driver.IgnoreHasLogic && !device.IsChildMPTOrMRO())
			{
				if (device.DeviceLogic.Clauses.Count == 0)
					Errors.Add(new DeviceValidationError(device, "Отсутствует логика срабатывания исполнительного устройства", ValidationErrorLevel.Warning));
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
			if (device.Driver.IsKauOrRSR2Kau)
			{
				if (device.Children.Count <= 1)
					Errors.Add(new DeviceValidationError(device, "Устройство должно содержать подключенные устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidatePumpAddresses(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.Pump)
			{
				if (!((device.IntAddress > 0 && device.IntAddress <= 8) || device.IntAddress == 12 || device.IntAddress == 14))
					Errors.Add(new DeviceValidationError(device, "Адрес устройства должен быть в диапазоне 1 - 8, 12, 14", ValidationErrorLevel.CannotWrite));
			}
		}

		static void ValidateParametersMinMax(XDevice device)
		{
			foreach (var property in device.Properties)
			{
				var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null)
				{
					if (driverProperty.Min != 0)
						if (property.Value < driverProperty.Min)
							Errors.Add(new DeviceValidationError(device, "Парметр " + driverProperty.Caption + " должен быть больше " + driverProperty.Min.ToString(), ValidationErrorLevel.CannotWrite));

					if (driverProperty.Max != 0)
						if (property.Value > driverProperty.Max)
							Errors.Add(new DeviceValidationError(device, "Парметр " + driverProperty.Caption + " должен быть меньше " + driverProperty.Max.ToString(), ValidationErrorLevel.CannotWrite));
				}
			}
		}

		static void ValidateNotUsedLogic(XDevice device)
		{
			foreach (var clause in device.DeviceLogic.Clauses)
			{
				foreach (var clauseDevices in clause.Devices)
				{
					if (clauseDevices.IsNotUsed)
						Errors.Add(new DeviceValidationError(device, "В логике задействованы неиспользуемые устройства", ValidationErrorLevel.CannotSave));
				}
			}
		}

		static void ValidateAddressEquality(XDevice device)
		{
			var addresses = new List<int>();
			foreach (var childDevice in DeviceValidationHelper.GetAllChildrenForDevice(device))
			{
				if (childDevice.Driver.HasAddress)
				{
					if (addresses.Contains(childDevice.ShleifNoNew * 256 + childDevice.IntAddress))
						Errors.Add(new DeviceValidationError(childDevice, string.Format("Дублируется адрес устройства"), ValidationErrorLevel.CannotWrite));
					else
						addresses.Add(childDevice.ShleifNoNew * 256 + childDevice.IntAddress);
				}
			}
		}

		static void ValidateDeviceRangeAddress(XDevice device)
		{
			if (device.Driver.IsGroupDevice)
			{
				if (device.Children.Any(x => x.IntAddress < device.IntAddress || (x.IntAddress - device.IntAddress) > device.Driver.GroupDeviceChildrenCount))
					Errors.Add(new DeviceValidationError(device, string.Format("Для всех подключенных устройтв необходимо выбрать адрес из диапазона: {0}", device.PresentationAddress), ValidationErrorLevel.Warning));
			}
		}

		static void ValidateRSR2AddressFollowing(XDevice device)
		{
			if (device.Driver.DriverType == XDriverType.RSR2_KAU)
			{
				foreach (var shleifDevice in device.Children)
				{
					var realChldren = XManager.GetAllDeviceChildren(shleifDevice);
					realChldren.RemoveAll(x => x.Driver.IsGroupDevice);
					for (int i = 0; i < realChldren.Count(); i++)
					{
						var realDevice = realChldren[i];
						if (realDevice.IntAddress != i + 1)
						{
							Errors.Add(new DeviceValidationError(realDevice, string.Format("Последовательность адресов шлейфа " + shleifDevice.IntAddress + " должна быть неразрывна начиная с 1"), ValidationErrorLevel.CannotWrite));
							break;
						}
					}
				}
			}
		}
	}

	static class DeviceValidationHelper
	{
		static List<XDevice> allChildren;
		public static List<XDevice> GetAllChildrenForDevice(XDevice device)
		{
			allChildren = new List<XDevice>();
			AddChild(device);
			return allChildren;
		}
		static void AddChild(XDevice device)
		{
			foreach (var child in device.Children)
			{
				allChildren.Add(child);
				if (child.Driver.DriverType == XDriverType.MPT || child.Driver.DriverType == XDriverType.MRO_2)
				{
					AddChild(child);
				}
			}
		}
	}
} 