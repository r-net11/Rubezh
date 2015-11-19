﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using RubezhAPI.GK;
using RubezhClient;
using GKProcessor;
using Infrastructure.Common;
using Infrastructure.Common.Validation;
using RubezhAPI;

namespace GKModule.Validation
{
	public partial class Validator
	{
		/// <summary>
		/// Валидация количества дескрипторов на каждом ГК и на каждом КАУ
		/// </summary>
		void ValidateGKObjectsCount()
		{
			var databases = new List<CommonDatabase>();
			databases.AddRange(DescriptorsManager.GkDatabases);
			databases.AddRange(DescriptorsManager.KauDatabases);

			foreach (var database in databases)
			{
				if (database is GkDatabase && database.Descriptors.Count > 65535)
					AddError(database.RootDevice, "Количество объектов на ГК превышает 65535", ValidationErrorLevel.CannotWrite);
				if (database is KauDatabase && database.Descriptors.Count > 2047)
					AddError(database.RootDevice, "Количество объектов на КАУ превышает 2047", ValidationErrorLevel.CannotWrite);
			}
		}

		void ValidateDevices()
		{
			ValidateAddressEquality();

			foreach (var device in GKManager.Devices)
			{
				if (device.IsNotUsed)
					continue;
				ValidateObjectOnlyOnOneGK(device);
				ValidateIPAddress(device);
				if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.DeviceNotConnected))
				{
					ValidateDeviceZone(device);
					ValidateGuardDeviceZone(device);
				}
				if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.DeviceHaveNoLogic))
					ValidateLogic(device);
				ValidateGKNotEmptyChildren(device);
				ValidateParametersMinMax(device);
				ValidateNotUsedLogic(device);
				ValidateRSR2AddressFollowing(device);
				ValidateKAUAddressFollowing(device);
				ValidateGuardDevice(device);
				ValidateDeviceIfInMPTAndDoor(device);
			}
		}

		/// <summary>
		/// Валидация уникальности адресов и выхода за пределы допустимогог диапазона адреса
		/// </summary>
		void ValidateAddressEquality()
		{
			var uids = new HashSet<Guid>();
			var deviceAddresses = new HashSet<string>();

			foreach (var device in GKManager.Devices)
			{
				if (!uids.Add(device.UID))
					AddError(device, "Дублируется идентификатор", ValidationErrorLevel.CannotSave);

				if (device.IsDisabled && device.Children.Count > 0)
				{
					AddError(device, "При кольцевой АЛС" + (device.IntAddress - 1) + "-" + device.IntAddress + " есть подключенные устройства на АЛС" + device.IntAddress, ValidationErrorLevel.CannotWrite);
				}

				if (device.DriverType == GKDriverType.System || device.DriverType == GKDriverType.GK || !device.Driver.HasAddress || device.Driver.IsAutoCreate || device.Driver.IsGroupDevice)
					continue;

				if (!deviceAddresses.Add(device.DottedPresentationAddress))
				{
					AddError(device, "Дублируется адрес устройства", ValidationErrorLevel.CannotWrite);
				}
				if ((device.Driver.MaxAddress > 0 && device.Driver.MinAddress > 0) && (device.IntAddress > device.Driver.MaxAddress || device.IntAddress < device.Driver.MinAddress))
				{
					AddError(device, "Неверно задан адрес. Диапазон допустимых значений от " + device.Driver.MinAddress.ToString() + " до " + device.Driver.MaxAddress.ToString(), ValidationErrorLevel.CannotWrite);
				}
			}
		}

		/// <summary>
		/// Валидация того, что для ГК верно задан IP-адрес
		/// </summary>
		/// <param name="device"></param>
		void ValidateIPAddress(GKDevice device)
		{
			if (!GKManager.IsValidIpAddress(device))
			{
				AddError(device, "Не верно задан IP адрес", ValidationErrorLevel.CannotWrite);
			}
		}

		void ValidateDeviceZone(GKDevice device)
		{
			if (device.IsInMPT)
				return;

			if (device.DriverType == GKDriverType.RSR2_AM_1)
			{
				if (device.Properties.Any(x => x.Name == "Сообщение для нормы" || x.Name == "Сообщение для сработки 1" || x.Name == "Сообщение для сработки 2"))
					return;
			}

			if (device.Driver.HasZone && device.Driver.HasGuardZone)
			{
				if (device.Zones.Count == 0 && device.GuardZones.Count == 0)
					AddError(device, "Устройство не подключено к зоне", ValidationErrorLevel.Warning);
			}

			else if (device.Driver.HasGuardZone )
			{
				if (device.GuardZones == null || device.GuardZones.Count == 0)
				{
					AddError(device, string.Format("Устройство не подключено к зоне"), ValidationErrorLevel.Warning);
				}
			}

			else if (device.Driver.HasZone)
			{
				if (device.GuardZones == null || device.GuardZones.Count == 0)
				{
					AddError(device, string.Format("Устройство не подключено к зоне"), ValidationErrorLevel.Warning);
				}
			}
		}

		void ValidateLogic(GKDevice device)
		{
			if (device.IsInMPT)
				return;

			if (device.DriverType == GKDriverType.GKRele)
				return;

			if (device.Driver.HasLogic && !device.Driver.IgnoreHasLogic && !GKManager.Doors.Any(x=> x.LockDevice == device || x.LockDeviceExit == device))
			{
				if (device.Logic.OnClausesGroup.Clauses.Count == 0)
					AddError(device, "Отсутствует логика срабатывания исполнительного устройства", ValidationErrorLevel.Warning);
			}
		}

		void ValidateGKNotEmptyChildren(GKDevice device)
		{
			if (device.DriverType == GKDriverType.GK)
			{
				if (device.Children.Where(x => x.Driver.IsKau).Count() == 0)
					AddError(device, "ГК должен содержать подключенные КАУ", ValidationErrorLevel.CannotWrite);
			}
		}

		void ValidateParametersMinMax(GKDevice device)
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
							AddError(device, "Параметр " + driverProperty.Caption + " должен быть больше " + minValue.ToString(), ValidationErrorLevel.CannotWrite);

					if (driverProperty.Max != 0)
						if (property.Value > driverProperty.Max)
							AddError(device, "Параметр " + driverProperty.Caption + " должен быть меньше " + maxValue.ToString(), ValidationErrorLevel.CannotWrite);
				}
			}
		}

		void ValidateNotUsedLogic(GKDevice device)
		{
			foreach (var clause in device.Logic.OnClausesGroup.Clauses)
			{
				foreach (var clauseDevices in clause.Devices)
				{
					if (clauseDevices.IsNotUsed)
						AddError(device, "В логике задействованы неиспользуемые устройства", ValidationErrorLevel.CannotSave);
				}
			}
		}

		void ValidateDeviceRangeAddress(GKDevice device)
		{
			if (device.Driver.IsGroupDevice)
			{
				if (device.Children.Any(x => x.IntAddress < device.IntAddress || (x.IntAddress - device.IntAddress) > device.Driver.GroupDeviceChildrenCount))
					AddError(device, string.Format("Для всех подключенных устройтв необходимо выбрать адрес из диапазона: {0}", device.PresentationAddress), ValidationErrorLevel.Warning);
			}
		}

		void ValidateRSR2AddressFollowing(GKDevice device)
		{
			if (device.DriverType == GKDriverType.RSR2_KAU)
			{
				foreach (var shleifDevice in device.Children)
				{
					if (shleifDevice.DriverType == GKDriverType.RSR2_KAU_Shleif)
					{
						var realChildren = shleifDevice.AllChildrenAndSelf;
						realChildren.RemoveAll(x => !x.IsRealDevice);
						for (int i = 0; i < realChildren.Count(); i++)
						{
							var realDevice = realChildren[i];
							if (realDevice.IntAddress != i + 1)
							{
								AddError(realDevice, string.Format("Последовательность адресов АЛС " + shleifDevice.IntAddress + " должна быть неразрывна начиная с 1"), ValidationErrorLevel.CannotWrite);
								break;
							}
						}
					}
				}
			}
		}

		void ValidateDeviceIfInMPTAndDoor(GKDevice device)
		{
			if (device.IsInMPT && device.Door != null)
			{
				var mpt = GKManager.MPTs.FirstOrDefault(x => x.MPTDevices.Any(y => y.DeviceUID == device.UID && (y.MPTDeviceType == GKMPTDeviceType.Bomb || y.MPTDeviceType == GKMPTDeviceType.DoNotEnterBoard || y.MPTDeviceType == GKMPTDeviceType.ExitBoard || y.MPTDeviceType == GKMPTDeviceType.AutomaticOffBoard)));
				 if(mpt!= null && (device.Door.LockDeviceUID == device.UID || device.Door.LockDeviceExitUID == device.UID ))
				{
					Errors.Add(new DeviceValidationError(device, string.Format("Устройство {0} не может учавствовать одновременно в {1} в качестве замка и в {2} в качестве {3}",device.PresentationName,device.Door.PresentationName, mpt.PresentationName , mpt.MPTDevices.FirstOrDefault(x=> x.DeviceUID == device.UID).MPTDeviceType.ToDescription()), ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateKAUAddressFollowing(GKDevice device)
		{
			if (device.DriverType == GKDriverType.GK)
			{
				var kauChildren1 = device.Children.Where(x => x.Driver.IsKau && GKManager.GetKauLine(x) == 0).ToList();
				ValidateKAUAddressFollowingInOneLine(kauChildren1);

				var kauChildren2 = device.Children.Where(x => x.Driver.IsKau && GKManager.GetKauLine(x) == 1).ToList();
				ValidateKAUAddressFollowingInOneLine(kauChildren2);
			}
		}

		void ValidateKAUAddressFollowingInOneLine(List<GKDevice> kauChildren)
		{
			for (int i = 0; i < kauChildren.Count; i++)
			{
				var kauDevice = kauChildren[i];
				if (kauDevice.IntAddress != i + 1)
				{
					AddError(kauDevice, string.Format("Последовательность адресов КАУ, подключенных к ГК, должна быть неразрывна начиная с 1"), ValidationErrorLevel.CannotWrite);
					break;
				}
			}
		}

		void ValidateGuardDevice(GKDevice device)
		{
			if (device.Driver.IsAm && device.Zones.Count > 0)
			{
				if (device.GuardZones.Any(x => x.GuardZoneDevices.Any(y => y.ActionType == GKGuardZoneDeviceActionType.SetAlarm && y.DeviceUID == device.UID)))
					AddError(device, string.Format("Тревожный датчик участвует сразу в охранной и пожарной зоне"), ValidationErrorLevel.Warning);
			}
		}

		void ValidateGuardDeviceZone(GKDevice device)
		{
			if (device.DriverType == GKDriverType.RSR2_GuardDetector || device.DriverType == GKDriverType.RSR2_GuardDetectorSound)
			{
				if (device.GuardZones == null || device.GuardZones.Count == 0)
				{
					AddError(device, string.Format("Охранное устройство не участвует в охранной зоне"), ValidationErrorLevel.Warning);
				}
			}
		}
	}
}