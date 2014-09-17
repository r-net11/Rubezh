using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using FiresecAPI.GK;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Validation;

namespace GKModule.Validation
{
	public partial class Validator
	{
		void ValidateDevices()
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
				ValidateParametersMinMax(device);
				ValidateNotUsedLogic(device);
				ValidateDeviceSelfLogic(device);
				ValidateRSR2AddressFollowing(device);
				ValidateKAUAddressFollowing(device);
				ValidateGuardDevice(device);
			}
		}

		bool MustValidate(string errorName)
		{
			return GlobalSettingsHelper.GlobalSettings.IgnoredErrors == null || !GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(errorName);
		}

		void ValidateAddressEquality()
		{
			var deviceAddresses = new HashSet<string>();
			foreach (var device in XManager.Devices)
			{
				if (device.DriverType == XDriverType.System || device.DriverType == XDriverType.GK || !device.Driver.HasAddress || device.Driver.IsAutoCreate || device.Driver.IsGroupDevice)
					continue;

				if (!deviceAddresses.Add(device.DottedAddress))
				{
					Errors.Add(new DeviceValidationError(device, "Дублируется адрес устройства", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateIPAddress(XDevice device)
		{
			if (!XManager.IsValidIpAddress(device))
			{
				Errors.Add(new DeviceValidationError(device, "Не верно задан IP адрес", ValidationErrorLevel.CannotWrite));
			}
		}

		bool CheckIpAddress(string ipAddress)
		{
			if (String.IsNullOrEmpty(ipAddress))
				return false;
			IPAddress address;
			return IPAddress.TryParse(ipAddress, out address);
		}

		void ValidateDifferentGK(XDevice device)
		{
			foreach (var clause in device.DeviceLogic.ClausesGroup.Clauses)
			{
				foreach (var clauseDevice in clause.Devices)
				{
					if (device.GKParent != null && clauseDevice.GKParent != null && device.GKParent != clauseDevice.GKParent)
						Errors.Add(new DeviceValidationError(device, "Логика сработки содержит устройства разных ГК", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateDeviceZone(XDevice device)
		{
			if (device.IsInMPT)
				return;

			if (device.DriverType == XDriverType.RSR2_AM_1)
			{
				if (device.Properties.Any(x => x.Name == "Сообщение для нормы" || x.Name == "Сообщение для сработки 1" || x.Name == "Сообщение для сработки 2"))
					return;
			}

			if (device.Driver.HasZone)
			{
				if (device.Zones.Count == 0)
					Errors.Add(new DeviceValidationError(device, "Устройство не подключено к зоне", ValidationErrorLevel.Warning));
			}
		}

		void ValidateDeviceLogic(XDevice device)
		{
			if(device.IsInMPT)
				return;

			if (device.DriverType == XDriverType.GKRele)
				return;

			if (device.Driver.HasLogic && !device.Driver.IgnoreHasLogic && !device.IsChildMPTOrMRO())
			{
				if (device.DeviceLogic.ClausesGroup.Clauses.Count == 0)
					Errors.Add(new DeviceValidationError(device, "Отсутствует логика срабатывания исполнительного устройства", ValidationErrorLevel.Warning));
			}
		}

		void ValidateGKNotEmptyChildren(XDevice device)
		{
			if (device.DriverType == XDriverType.GK)
			{
				if (device.Children.Count <= 14)
					Errors.Add(new DeviceValidationError(device, "Устройство должно содержать подключенные устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateKAUNotEmptyChildren(XDevice device)
		{
			if (device.Driver.IsKauOrRSR2Kau)
			{
				if (device.Children.Count <= 1)
					Errors.Add(new DeviceValidationError(device, "Устройство должно содержать подключенные устройства", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateParametersMinMax(XDevice device)
		{
			if (device.IsInMPT)
				return;

			foreach (var property in device.Properties)
			{
				var driverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				if (driverProperty != null)
				{
					double minValue = driverProperty.Min;
					double maxValue = driverProperty.Max;
					if (driverProperty.Multiplier != 0)
					{
						minValue /= driverProperty.Multiplier;
						maxValue /= driverProperty.Multiplier;
					}
					if (driverProperty.Min != 0)
						if (property.Value < driverProperty.Min)
							Errors.Add(new DeviceValidationError(device, "Параметр " + driverProperty.Caption + " должен быть больше " + minValue.ToString(), ValidationErrorLevel.CannotWrite));

					if (driverProperty.Max != 0)
						if (property.Value > driverProperty.Max)
							Errors.Add(new DeviceValidationError(device, "Параметр " + driverProperty.Caption + " должен быть меньше " + maxValue.ToString(), ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateNotUsedLogic(XDevice device)
		{
			foreach (var clause in device.DeviceLogic.ClausesGroup.Clauses)
			{
				foreach (var clauseDevices in clause.Devices)
				{
					if (clauseDevices.IsNotUsed)
						Errors.Add(new DeviceValidationError(device, "В логике задействованы неиспользуемые устройства", ValidationErrorLevel.CannotSave));
				}
			}
		}

		void ValidateDeviceSelfLogic(XDevice device)
		{
			if (device.ClauseInputDevices.Contains(device))
				Errors.Add(new DeviceValidationError(device, "Устройство зависит от самого себя", ValidationErrorLevel.CannotWrite));
		}

		void ValidateDeviceRangeAddress(XDevice device)
		{
			if (device.Driver.IsGroupDevice)
			{
				if (device.Children.Any(x => x.IntAddress < device.IntAddress || (x.IntAddress - device.IntAddress) > device.Driver.GroupDeviceChildrenCount))
					Errors.Add(new DeviceValidationError(device, string.Format("Для всех подключенных устройтв необходимо выбрать адрес из диапазона: {0}", device.PresentationAddress), ValidationErrorLevel.Warning));
			}
		}

		void ValidateRSR2AddressFollowing(XDevice device)
		{
			if (device.DriverType == XDriverType.RSR2_KAU)
			{
				foreach (var shleifDevice in device.Children)
				{
					if (shleifDevice.DriverType == XDriverType.RSR2_KAU_Shleif)
					{
						var realChildren = shleifDevice.AllChildrenAndSelf;
						realChildren.RemoveAll(x => !x.IsRealDevice);
						for (int i = 0; i < realChildren.Count(); i++)
						{
							var realDevice = realChildren[i];
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

		void ValidateKAUAddressFollowing(XDevice device)
		{
			if (device.DriverType == XDriverType.GK)
			{
				var kauChildren1 = device.Children.Where(x => x.Driver.IsKauOrRSR2Kau && XManager.GetKauLine(x) == 0).ToList();
				ValidateKAUAddressFollowingInOneLine(kauChildren1);

				var kauChildren2 = device.Children.Where(x => x.Driver.IsKauOrRSR2Kau && XManager.GetKauLine(x) == 1).ToList();
				ValidateKAUAddressFollowingInOneLine(kauChildren2);
			}
		}

		void ValidateKAUAddressFollowingInOneLine(List<XDevice> kauChildren)
		{
			for (int i = 0; i < kauChildren.Count; i++)
			{
				var kauDevice = kauChildren[i];
				if (kauDevice.IntAddress != i + 1)
				{
					Errors.Add(new DeviceValidationError(kauDevice, string.Format("Последовательность адресов КАУ, подключенных к ГК, должна быть неразрывна начиная с 1"), ValidationErrorLevel.CannotWrite));
					break;
				}
			}
		}

		void ValidateGuardDevice(XDevice device)
		{
			if (device.DriverType == XDriverType.RSR2_GuardDetector)
			{
				if (device.GuardZone == null)
				{
					Errors.Add(new DeviceValidationError(device, string.Format("Охранное устройство не участвует в охранной зоне"), ValidationErrorLevel.Warning));
				}
			}
		}
	}
}