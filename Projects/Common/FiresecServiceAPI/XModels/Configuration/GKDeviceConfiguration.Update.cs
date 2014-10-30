using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecClient;

namespace FiresecAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void UpdateConfiguration()
		{
			if (RootDevice == null)
			{
				var systemDriver = GKManager.Drivers.FirstOrDefault(x => x.DriverType == GKDriverType.System);
				if (systemDriver != null)
				{
					RootDevice = new GKDevice()
					{
						DriverUID = systemDriver.UID,
						Driver = systemDriver
					};
				}
				else
				{
					Logger.Error("GKManager.SetEmptyConfiguration systemDriver = null");
				}
			}
			ValidateVersion();

			Update();
			foreach (var device in Devices)
			{
				device.Driver = GKManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					//MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
			}
			Reorder();

			InitializeProperties();
			Invalidate();
			CopyMPTProperties();
		}

		void InitializeProperties()
		{
			foreach (var device in Devices)
			{
				if (device.Properties == null)
					device.Properties = new List<GKProperty>();
				foreach (var property in device.Properties)
				{
					property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				}
				device.Properties.RemoveAll(x => x.DriverProperty == null);

				foreach (var property in device.DeviceProperties)
				{
					property.DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == property.Name);
				}
				device.DeviceProperties.RemoveAll(x => x.DriverProperty == null);
				device.InitializeDefaultProperties();
			}
		}

		void Invalidate()
		{
			ClearAllReferences();
			InitializeDevicesInZone();
			InitializeLogic();
			InitializeDirections();
			InitializePumpStations();
			InitializeMPTs();
			InitializeDelays();
			InitializeGuardZones();
			InitializeCodes();
			InitializeDoors();
			InitializeSchedules();
			UpdateGKChildrenDescription();
		}

		void ClearAllReferences()
		{
			foreach (var device in Devices)
			{
				device.ClearClauseDependencies();
				device.Zones = new List<GKZone>();
				device.Directions = new List<GKDirection>();
				device.GuardZone = null;
			}
			foreach (var zone in Zones)
			{
				zone.ClearClauseDependencies();
				zone.Devices = new List<GKDevice>();
				zone.Directions = new List<GKDirection>();
				zone.DevicesInLogic = new List<GKDevice>();
			}
			foreach (var direction in Directions)
			{
				direction.ClearClauseDependencies();
				direction.InputZones = new List<GKZone>();
				direction.InputDevices = new List<GKDevice>();
				direction.OutputDevices = new List<GKDevice>();
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.ClearClauseDependencies();
				pumpStation.NSDevices = new List<GKDevice>();
			}
			foreach (var mpt in MPTs)
			{
				mpt.ClearClauseDependencies();
				mpt.Devices = new List<GKDevice>();
			}
			foreach (var delay in Delays)
			{
				delay.ClearClauseDependencies();
			}
		}

		void InitializeDevicesInZone()
		{
			foreach (var device in Devices)
			{
				var zoneUIDs = new List<Guid>();
				if (device.Driver.HasZone)
				{
					foreach (var zoneUID in device.ZoneUIDs)
					{
						var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
						if (zone != null)
						{
							zoneUIDs.Add(zoneUID);
							device.Zones.Add(zone);
							zone.Devices.Add(device);
						}
					}
				}
				device.ZoneUIDs = zoneUIDs;
			}
		}

		void InitializeLogic()
		{
			foreach (var device in Devices)
			{
				InvalidateOneLogic(device, device.DeviceLogic);
				if (device.NSLogic != null)
					InvalidateOneLogic(device, device.NSLogic);
			}
		}

		public void InvalidateOneLogic(GKDevice device, GKDeviceLogic deviceLogic)
		{
			InvalidateInputObjectsBaseLogic(device, deviceLogic);
			foreach (var clause in deviceLogic.ClausesGroup.Clauses)
			{
				foreach (var clauseZone in clause.Zones)
				{
					clauseZone.DevicesInLogic.Add(device);
				}
				foreach (var clauseDirection in clause.Directions)
				{
					clauseDirection.OutputDevices.Add(device);
					device.Directions.Add(clauseDirection);
				}
			}
			foreach (var clause in device.DeviceLogic.OffClausesGroup.Clauses)
			{
				foreach (var clauseZone in clause.Zones)
				{
					clauseZone.DevicesInLogic.Add(device);
				}
				foreach (var clauseDirection in clause.Directions)
				{
					clauseDirection.OutputDevices.Add(device);
					device.Directions.Add(clauseDirection);
				}
			}
		}

		void InitializeDirections()
		{
			foreach (var direction in Directions)
			{
				var directionDevices = new List<GKDirectionDevice>();
				foreach (var directionDevice in direction.DirectionDevices)
				{
					if (directionDevice.DeviceUID != Guid.Empty)
					{
						var device = Devices.FirstOrDefault(x => x.UID == directionDevice.DeviceUID);
						directionDevice.Device = device;
						if (device != null)
						{
							directionDevices.Add(directionDevice);
							direction.InputDevices.Add(device);
							device.Directions.Add(direction);
						}
					}
				}
				direction.DirectionDevices = directionDevices;

				var directionZones = new List<GKDirectionZone>();
				foreach (var directionZone in direction.DirectionZones)
				{
					if (directionZone.ZoneUID != Guid.Empty)
					{
						var zone = Zones.FirstOrDefault(x => x.UID == directionZone.ZoneUID);
						directionZone.Zone = zone;
						if (zone != null)
						{
							directionZones.Add(directionZone);
							direction.InputZones.Add(zone);
							zone.Directions.Add(direction);
						}
					}
				}
				direction.DirectionZones = directionZones;
			}
		}

		void InitializePumpStations()
		{
			foreach (var pumpStation in PumpStations)
			{
				var nsDeviceUIDs = new List<Guid>();
				foreach (var nsDeviceUID in pumpStation.NSDeviceUIDs)
				{
					var device = Devices.FirstOrDefault(x => x.UID == nsDeviceUID);
					if (device != null)
					{
						if (device.Driver.DriverType == GKDriverType.RSR2_Bush_Drenazh || device.Driver.DriverType == GKDriverType.RSR2_Bush_Jokey || device.Driver.DriverType == GKDriverType.RSR2_Bush_Fire)
						{
							nsDeviceUIDs.Add(device.UID);
							pumpStation.NSDevices.Add(device);
						}
					}
				}
				pumpStation.NSDeviceUIDs = nsDeviceUIDs;
				InvalidateInputObjectsBaseLogic(pumpStation, pumpStation.StartLogic);
				InvalidateInputObjectsBaseLogic(pumpStation, pumpStation.StopLogic);
				InvalidateInputObjectsBaseLogic(pumpStation, pumpStation.AutomaticOffLogic);
			}
		}

		void InitializeMPTs()
		{
			foreach (var mpt in MPTs)
			{
				InvalidateInputObjectsBaseLogic(mpt, mpt.StartLogic);

				var mptDevices = new List<GKMPTDevice>();
				foreach (var mptDevice in mpt.MPTDevices)
				{
					var device = Devices.FirstOrDefault(x => x.UID == mptDevice.DeviceUID);
					if (device != null && GKMPTDevice.GetAvailableMPTDriverTypes(mptDevice.MPTDeviceType).Contains(device.DriverType))
					{
						mptDevice.Device = device;
						mptDevices.Add(mptDevice);
						device.IsInMPT = true;
						mpt.Devices.Add(device);
					}
				}
				mpt.MPTDevices = mptDevices;
			}
		}

		void InitializeDelays()
		{
			foreach (var delay in Delays)
			{
				InvalidateInputObjectsBaseLogic(delay, delay.DeviceLogic);
			}
		}

		public void InvalidateInputObjectsBaseLogic(GKBase gkBase, GKDeviceLogic deviceLogic)
		{
			deviceLogic.ClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, deviceLogic.ClausesGroup);
			deviceLogic.OffClausesGroup = InvalidateOneInputObjectsBaseLogic(gkBase, deviceLogic.OffClausesGroup);
		}

		public GKClauseGroup InvalidateOneInputObjectsBaseLogic(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			var result = new GKClauseGroup();
			result.ClauseJounOperationType = clauseGroup.ClauseJounOperationType;
			var groups = new List<GKClauseGroup>();
			foreach (var group in clauseGroup.ClauseGroups)
			{
				var _clauseGroup = InvalidateOneInputObjectsBaseLogic(gkBase, group);
				if (_clauseGroup.Clauses.Count + _clauseGroup.ClauseGroups.Count > 0)
					groups.Add(_clauseGroup);
			}
			result.ClauseGroups = groups;

			foreach (var clause in clauseGroup.Clauses)
			{
				clause.Devices = new List<GKDevice>();
				clause.Zones = new List<GKZone>();
				clause.GuardZones = new List<GKGuardZone>();
				clause.Directions = new List<GKDirection>();
				clause.MPTs = new List<GKMPT>();
				clause.Delays = new List<GKDelay>();

				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in clause.DeviceUIDs)
				{
					var clauseDevice = Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (clauseDevice != null && !clauseDevice.IsNotUsed)
					{
						deviceUIDs.Add(deviceUID);
						clause.Devices.Add(clauseDevice);
						if (!gkBase.ClauseInputDevices.Contains(clauseDevice))
							gkBase.ClauseInputDevices.Add(clauseDevice);
					}
				}
				clause.DeviceUIDs = deviceUIDs;

				var zoneUIDs = new List<Guid>();
				foreach (var zoneUID in clause.ZoneUIDs)
				{
					var zone = Zones.FirstOrDefault(x => x.UID == zoneUID);
					if (zone != null)
					{
						zoneUIDs.Add(zoneUID);
						clause.Zones.Add(zone);
						if (!gkBase.ClauseInputZones.Contains(zone))
							gkBase.ClauseInputZones.Add(zone);
					}
				}
				clause.ZoneUIDs = zoneUIDs;

				var guardZoneUIDs = new List<Guid>();
				foreach (var guardZoneUID in clause.GuardZoneUIDs)
				{
					var guardZone = GuardZones.FirstOrDefault(x => x.UID == guardZoneUID);
					if (guardZone != null)
					{
						guardZoneUIDs.Add(guardZoneUID);
						clause.GuardZones.Add(guardZone);
						if (!gkBase.ClauseInputGuardZones.Contains(guardZone))
							gkBase.ClauseInputGuardZones.Add(guardZone);
					}
				}
				clause.GuardZoneUIDs = guardZoneUIDs;

				var directionUIDs = new List<Guid>();
				foreach (var directionUID in clause.DirectionUIDs)
				{
					var direction = Directions.FirstOrDefault(x => x.UID == directionUID);
					if (direction != null)
					{
						directionUIDs.Add(directionUID);
						clause.Directions.Add(direction);
						if (!gkBase.ClauseInputDirections.Contains(direction))
							gkBase.ClauseInputDirections.Add(direction);
					}
				}
				clause.DirectionUIDs = directionUIDs;

				var mptUIDs = new List<Guid>();
				foreach (var mptUID in clause.MPTUIDs)
				{
					var mpt = MPTs.FirstOrDefault(x => x.UID == mptUID);
					if (mpt != null)
					{
						mptUIDs.Add(mptUID);
						clause.MPTs.Add(mpt);
						if (!gkBase.ClauseInputMPTs.Contains(mpt))
							gkBase.ClauseInputMPTs.Add(mpt);
					}
				}
				clause.MPTUIDs = mptUIDs;

				var delayUIDs = new List<Guid>();
				foreach (var delayUID in clause.DelayUIDs)
				{
					var delay = Delays.FirstOrDefault(x => x.UID == delayUID);
					if (delay != null)
					{
						delayUIDs.Add(delayUID);
						clause.Delays.Add(delay);
						if (!gkBase.ClauseInputDelays.Contains(delay))
							gkBase.ClauseInputDelays.Add(delay);
					}
				}
				clause.DelayUIDs = delayUIDs;

				if (clause.HasObjects())
					result.Clauses.Add(clause);
			}
			return result;
		}

		void InitializeGuardZones()
		{
			foreach (var guardZone in GuardZones)
			{
				var guardZoneDevices = new List<GKGuardZoneDevice>();
				foreach (var guardZoneDevice in guardZone.GuardZoneDevices)
				{
					var device = Devices.FirstOrDefault(x => x.UID == guardZoneDevice.DeviceUID);
					if (device != null)
					{
						if (device.DriverType == GKDriverType.RSR2_GuardDetector || device.DriverType == GKDriverType.RSR2_AM_1 || device.DriverType == GKDriverType.RSR2_CodeReader)
						{
							guardZoneDevice.Device = device;
							guardZoneDevices.Add(guardZoneDevice);
							device.GuardZone = guardZone;
						}
					}
				}
				guardZone.GuardZoneDevices = guardZoneDevices;
			}
		}

		void InitializeCodes()
		{
			foreach (var code in Codes)
			{
			}
		}

		void InitializeDoors()
		{
			foreach (var door in Doors)
			{
				door.EnterDevice = Devices.FirstOrDefault(x => x.UID == door.EnterDeviceUID);
				if (door.EnterDevice == null)
					door.EnterDeviceUID = Guid.Empty;

				door.ExitDevice = Devices.FirstOrDefault(x => x.UID == door.ExitDeviceUID);
				if (door.ExitDevice == null)
					door.ExitDeviceUID = Guid.Empty;

				door.LockDevice = Devices.FirstOrDefault(x => x.UID == door.LockDeviceUID);
				if (door.LockDevice == null)
					door.LockDeviceUID = Guid.Empty;

				door.LockControlDevice = Devices.FirstOrDefault(x => x.UID == door.LockControlDeviceUID);
				if (door.LockControlDevice == null)
					door.LockControlDeviceUID = Guid.Empty;
			}
		}

		void InitializeSchedules()
		{
			var neverDaySchedule = DaySchedules.FirstOrDefault(x => x.Name == "<Никогда>");
			if (neverDaySchedule == null)
			{
				neverDaySchedule = new GKDaySchedule();
				neverDaySchedule.Name = "<Никогда>";
				DaySchedules.Add(neverDaySchedule);
			}

			var alwaysDaySchedule = DaySchedules.FirstOrDefault(x => x.Name == "<Всегда>");
			if (alwaysDaySchedule == null)
			{
				alwaysDaySchedule = new GKDaySchedule();
				alwaysDaySchedule.Name = "<Всегда>";
				alwaysDaySchedule.DayScheduleParts.Add(new GKDaySchedulePart() { StartMilliseconds = 0, EndMilliseconds = new TimeSpan(23, 59, 59).TotalMilliseconds });
				DaySchedules.Add(alwaysDaySchedule);
			}

			foreach (var schedule in Schedules)
			{
				var uids = new List<Guid>();
				foreach (var dayScheduleUID in schedule.DayScheduleUIDs)
				{
					var daySchedule = DaySchedules.FirstOrDefault(x => x.UID == dayScheduleUID);
					if (daySchedule != null)
					{
						uids.Add(dayScheduleUID);
					}
				}
				schedule.DayScheduleUIDs = uids;
			}
		}

		void UpdateGKChildrenDescription()
		{
			foreach (var gkControllerDevice in RootDevice.Children)
			{
				UpdateGKPredefinedName(gkControllerDevice);
			}
		}

		public void UpdateGKPredefinedName(GKDevice device)
		{
			if (device.DriverType == GKDriverType.GK && device.Children.Count >= 15)
			{
				device.Children[0].PredefinedName = "Неисправность";
				device.Children[1].PredefinedName = "Пожар 1";
				device.Children[2].PredefinedName = "Пожар 2";
				device.Children[3].PredefinedName = "Внимание";
				device.Children[4].PredefinedName = "Включение ПУСК";
				device.Children[5].PredefinedName = "Тест";
				device.Children[6].PredefinedName = "Отключение";
				device.Children[7].PredefinedName = "Автоматика отключена";
				device.Children[8].PredefinedName = "Звук отключен";
				device.Children[9].PredefinedName = "Останов пуска";
				device.Children[10].PredefinedName = "Реле 1";
				device.Children[11].PredefinedName = "Реле 2";
				device.Children[12].PredefinedName = "Реле 3";
				device.Children[13].PredefinedName = "Реле 4";
				device.Children[14].PredefinedName = "Реле 5";
			}
		}

		void CopyMPTProperties()
		{
			foreach (var mpt in MPTs)
			{
				foreach (var mptDevice in mpt.MPTDevices)
				{
					SetIsMPT(mptDevice);
				}
			}
		}

		public void SetIsMPT(GKMPTDevice mptDevice)
		{
			if (mptDevice.Device != null)
			{
				mptDevice.Device.IsInMPT = true;
				GKManager.ChangeDeviceLogic(mptDevice.Device, new GKDeviceLogic());
				mptDevice.Device.ZoneUIDs = new List<Guid>();
				mptDevice.Device.Zones.Clear();
			}
		}

		public void SetMPTDefaultProperty(GKDevice device)
		{
			if (device != null)
			{
				switch (device.DriverType)
				{
					case GKDriverType.RSR2_AM_1:
						SetDeviceProperty(device, "Конфигурация", 1);
						break;

					case GKDriverType.RSR2_OPS:
					case GKDriverType.RSR2_OPZ:
					case GKDriverType.RSR2_OPK:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Время удержания, с", 65000);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние для модуля Выключено", 0);
						SetDeviceProperty(device, "Состояние для режима Удержания", 4);
						SetDeviceProperty(device, "Состояние для режима Включено", 16);
						break;

					case GKDriverType.RSR2_MVK8:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Время удержания, с", 2);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Удержания", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
						SetDeviceProperty(device, "Контроль", 0);
						SetDeviceProperty(device, "Норма питания, 0.1В", 80);
						break;

					case GKDriverType.RSR2_RM_1:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Время удержания, с", 2);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Удержания", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
						break;
				}
			}
		}

		void SetDeviceProperty(GKDevice device, string propertyName, int value)
		{
			var property = device.Properties.FirstOrDefault(x => x.Name == propertyName);
			if (property == null)
			{
				property = new GKProperty()
				{
					Name = propertyName,
					DriverProperty = device.Driver.Properties.FirstOrDefault(x => x.Name == propertyName)
				};
				device.Properties.Add(property);
			}
			property.Value = (ushort)value;
		}
	}
}