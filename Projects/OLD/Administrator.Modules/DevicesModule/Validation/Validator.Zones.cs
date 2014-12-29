using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Common.Validation;
using Infrastructure.Common;

namespace DevicesModule.Validation
{
	partial class Validator
	{
		void ValidateZones()
		{
			ValidateGuardZonesCount();

			foreach (var zone in _firesecConfiguration.DeviceConfiguration.Zones)
			{
				if (zone.DevicesInZone.Count == 0)
				{
					Errors.Add(new ZoneValidationError(zone, "В зоне отсутствуют устройства", ValidationErrorLevel.Warning));
				}
				else
				{
					ValidateZoneType(zone);
					ValidateZoneOutDevices(zone);
					ValidateZoneSingleNS(zone);
					ValidateZoneSingleMPT(zone);
					ValidateMPTDetectorCount(zone);
					ValidateZoneDifferentLine(zone);
					ValidateZoneSingleBoltInDirectionZone(zone);
					ValidateGuardZoneHasDevicesFromSinglePanel(zone);
				}

				ValidateZoneHasDevicesFromDifferentNetworks(zone);
				ValidateZoneHasDifferentDevices(zone);
				ValidateZoneNumber(zone);
				ValidateZoneNameLength(zone);
				ValidateZoneDescriptionLength(zone);
				ValidateZoneName(zone);
			}
		}

		void ValidateZoneHasDevicesFromDifferentNetworks(Zone zone)
		{
			var deviceUIDs = new HashSet<Guid>();
			foreach (var device in zone.DevicesInZone)
			{
				deviceUIDs.Add(device.ParentChannel.UID);
			}
			foreach (var device in zone.DevicesInZoneLogic)
			{
				deviceUIDs.Add(device.ParentChannel.UID);
			}
			if (deviceUIDs.Count > 1)
				Errors.Add(new ZoneValidationError(zone, "В зоне указано устройство, находящееся в другой сети RS-485", ValidationErrorLevel.CannotWrite));
		}

		void ValidateZoneHasDifferentDevices(Zone zone)
		{
			if (zone.ZoneType == ZoneType.Guard)
			{
				var deviceUIDs = new HashSet<Guid>();
				foreach (var device in zone.DevicesInZone)
				{
					deviceUIDs.Add(device.ParentPanel.UID);
				}
				if (deviceUIDs.Count > 1)
					Errors.Add(new ZoneValidationError(zone, "В охранной зоне указано устройство, находящееся в другом приборе", ValidationErrorLevel.CannotWrite));
			}
		}

		void ValidateGuardZonesCount()
		{
			int guardZonesCount = 0;
			foreach (var zone in _firesecConfiguration.DeviceConfiguration.Zones)
			{
				if (zone.ZoneType == ZoneType.Guard)
					++guardZonesCount;
			}
			if (guardZonesCount > 64)
				Errors.Add(new CommonValidationError(ModuleType.Devices, "Зоны", string.Empty, string.Format("Превышено максимальное количество охранных зон ({0} из 64 максимально возможных)", guardZonesCount), ValidationErrorLevel.CannotWrite));
		}

		void ValidateZoneType(Zone zone)
		{
			switch (zone.ZoneType)
			{
				case ZoneType.Fire:
					var guardDevice = zone.DevicesInZone.FirstOrDefault(x => x.Driver.DeviceType == DeviceType.Sequrity);
					if (guardDevice != null)
						Errors.Add(new ZoneValidationError(zone, string.Format("В зону не может быть помещено охранное устройство ({0})", guardDevice.PresentationAddress), ValidationErrorLevel.CannotSave));
					break;

				case ZoneType.Guard:
					var fireDevice = zone.DevicesInZone.FirstOrDefault(x => x.Driver.DeviceType == DeviceType.Fire);
					if (fireDevice != null)
						Errors.Add(new ZoneValidationError(zone, string.Format("В зону не может быть помещено пожарное устройство ({0})", fireDevice.PresentationAddress), ValidationErrorLevel.CannotSave));
					break;
			}
		}

		void ValidateZoneOutDevices(Zone zone)
		{
			if (zone.DevicesInZone.All(x => x.Driver.IsOutDevice))
				Errors.Add(new ZoneValidationError(zone, "К зоне нельзя отнести только выходные устройства", ValidationErrorLevel.CannotWrite));
		}

		void ValidateZoneSingleNS(Zone zone)
		{
			if (zone.DevicesInZoneLogic.Where(x => x.Driver.DriverType == DriverType.PumpStation).Count() > 1)
				Errors.Add(new ZoneValidationError(zone, "В одной зоне не может быть несколько внешних НС", ValidationErrorLevel.CannotWrite));
		}

		void ValidateZoneSingleMPT(Zone zone)
		{
			var mptCount = 0;
			foreach (var device in zone.DevicesInZone)
			{
				if (device.Driver.DriverType == DriverType.MPT && !FiresecManager.FiresecConfiguration.IsChildMPT(device))
				{
					mptCount++;
				}
			}
			if (mptCount > 1)
				Errors.Add(new ZoneValidationError(zone, "В зоне не может быть несколько МПТ", ValidationErrorLevel.CannotWrite));
		}

		void ValidateMPTDetectorCount(Zone zone)
		{
			if (zone.DevicesInZone.Any(x => x.Driver.DriverType == DriverType.MPT))
			{
				if (zone.DevicesInZone.Any(x => x.Driver.DriverType == DriverType.HandDetector))
					return;
				var amDevices = zone.DevicesInZone.Where(x => x.Driver.DriverType == DriverType.AM_1 ||
					x.Driver.DriverType == DriverType.AMP_4);
				if (amDevices.Count() > 0)
				{
					if ((zone.ZoneType == ZoneType.Fire) && (zone.DetectorCount > amDevices.Count()))
						Errors.Add(new ZoneValidationError(zone, "Количество подключенных к зоне адресных меток меньше настроенного в зоне количества", ValidationErrorLevel.Warning));
				}
				else
				{
					var sensorDevices = GetSensorsInZone(zone);
					if (sensorDevices.Count < 2)
						Errors.Add(new ZoneValidationError(zone, "В зоне с МПТ не может быть менее двух извещателей", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateZoneDifferentLine(Zone zone)
		{
			var zoneAutoCreateDevices = zone.DevicesInZone.Where(x => x.Driver.IsAutoCreate && x.Driver.IsDeviceOnShleif).ToList();
			foreach (var device in zoneAutoCreateDevices)
			{
				var shliefCount = device.AddressFullPath.Substring(0, device.AddressFullPath.IndexOf('.'));
				if (shliefCount != "0" && zone.DevicesInZone.Any(x => x.AddressFullPath.Substring(0, x.AddressFullPath.IndexOf('.')) != shliefCount))
				{
					Errors.Add(new ZoneValidationError(zone, string.Format("Адрес встроенного устройства ({0}) в зоне не соответствует номерам шлейфа прочих устройств", device.PresentationAddress), ValidationErrorLevel.CannotWrite));
					return;
				}
			}
		}

		void ValidateZoneSingleBoltInDirectionZone(Zone zone)
		{
			if (zone.DevicesInZoneLogic.Count(x => x.Driver.DriverType == DriverType.Valve) > 1 && _firesecConfiguration.DeviceConfiguration.Directions.Any(x => x.ZoneUIDs.Contains(zone.UID)))
				Errors.Add(new ZoneValidationError(zone, "В зоне направления не может быть более одной задвижки", ValidationErrorLevel.CannotWrite));
		}

		void ValidateGuardZoneHasDevicesFromSinglePanel(Zone zone)
		{
			if (zone.ZoneType == ZoneType.Guard)
			{
				var panelDevices = new HashSet<Guid>();
				foreach (var device in zone.DevicesInZone)
				{
					if (device.Driver.DriverType == DriverType.AM1_O)
					{
						panelDevices.Add(device.ParentChannel.UID);
					}
				}
				if (panelDevices.Count > 1)
				{
					Errors.Add(new ZoneValidationError(zone, "В охранной зоне присутствуют устройства из разных приборов", ValidationErrorLevel.CannotWrite));
				}
			}
		}

		void ValidateZoneNameLength(Zone zone)
		{
			if (zone.Name != null && zone.Name.Length > 20)
				Errors.Add(new ZoneValidationError(zone, "Слишком длинное наименование зоны (более 20 символов)", ValidationErrorLevel.CannotWrite));
		}

		void ValidateZoneDescriptionLength(Zone zone)
		{
			if (zone.Description != null && zone.Description.Length > 256)
				Errors.Add(new ZoneValidationError(zone, "Слишком длинное примечание в зоне (более 256 символов)", ValidationErrorLevel.CannotSave));
		}

		void ValidateZoneNumber(Zone zone)
		{
			if (_firesecConfiguration.DeviceConfiguration.Zones.Where(x => x != zone).Any(x => x.No == zone.No))
				Errors.Add(new ZoneValidationError(zone, "Дублируется номер зоны", ValidationErrorLevel.CannotSave));
		}

		void ValidateZoneName(Zone zone)
		{
			if (string.IsNullOrWhiteSpace(zone.Name))
				Errors.Add(new ZoneValidationError(zone, "Не указано наименование зоны", ValidationErrorLevel.CannotWrite));
		}

		List<Device> GetSensorsInZone(Zone zone)
		{
			var sensorDevices = new List<Device>();
			foreach (var device in zone.DevicesInZone)
			{
				switch (device.Driver.DriverType)
				{
					case DriverType.SmokeDetector:
					case DriverType.HeatDetector:
					case DriverType.CombinedDetector:
					case DriverType.AM_1:
					case DriverType.AMP_4:
					case DriverType.RadioHandDetector:
					case DriverType.RadioSmokeDetector:
						sensorDevices.Add(device);
						break;
				}
			}
			return sensorDevices;
		}
	}
}