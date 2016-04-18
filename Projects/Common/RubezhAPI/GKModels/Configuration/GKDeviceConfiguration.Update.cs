using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using RubezhAPI;
using System.Diagnostics;

namespace RubezhAPI.GK
{
	public partial class GKDeviceConfiguration
	{
		public void UpdateConfiguration()
		{
			ValidateVersion();
			Update();

			foreach (var device in Devices)
			{
				device.Driver = GKManager.Drivers.FirstOrDefault(x => x.UID == device.DriverUID);
				if (device.Driver == null)
				{
					//MessageBoxService.Show("Ошибка при сопоставлении драйвера устройств ГК");
				}
				if (device.DriverType == GKDriverType.GK)
					device.IntAddress = 0;
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
			InitializeLogic();
			InitializeCodes();
			InitializeOPC();
			UpdateGKChildrenDescription();
			Invalidation();
		}

		void ClearAllReferences()
		{
			foreach (var device in Devices)
			{
				device.ClearClauseDependencies();
				device.Zones = new List<GKZone>();
				device.GuardZones = new List<GKGuardZone>();
				if (device.GKReflectionItem != null)
				{
					device.GKReflectionItem.Delays = new List<GKDelay>();
					device.GKReflectionItem.Devices = new List<GKDevice>();
					device.GKReflectionItem.Diretions = new List<GKDirection>();
					device.GKReflectionItem.GuardZones = new List<GKGuardZone>();
					device.GKReflectionItem.NSs = new List<GKPumpStation>();
					device.GKReflectionItem.Zones = new List<GKZone>();
					device.GKReflectionItem.MPTs = new List<GKMPT>();
					device.Door = null;
				}
			}
			foreach (var guardZone in GuardZones)
			{
				guardZone.ClearClauseDependencies();
			}
			 foreach (var zone in Zones)
			{
				zone.ClearClauseDependencies();
				zone.Devices = new List<GKDevice>();
			}
			foreach (var direction in Directions)
			{
				direction.ClearClauseDependencies();
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.ClearClauseDependencies();
				pumpStation.Pim.ClearClauseDependencies();
				pumpStation.NSDevices = new List<GKDevice>();
			}
			foreach (var mpt in MPTs)
			{
				mpt.ClearClauseDependencies();
			}
			foreach (var delay in Delays)
			{
				delay.ClearClauseDependencies();
			}
		}

		void InitializeLogic()
		{
			foreach (var device in Devices)
			{
				InvalidateOneLogic(device, device.Logic);
				if (device.NSLogic != null)
					InvalidateOneLogic(device, device.NSLogic);
			}
		}

		void Invalidation()
		{
			Devices.ForEach(x => x.Invalidate(this));

			MPTs.ForEach(x => x.Invalidate(this));
			
			Doors.ForEach(x => x.Invalidate(this));

			PumpStations.ForEach(x => x.Invalidate(this));

			GuardZones.ForEach(x => x.Invalidate(this));

			Delays.ForEach(x => x.Invalidate(this));

			Directions.ForEach(x => x.Invalidate(this));
		}

		public void InvalidateOneLogic(GKBase gkBase, GKLogic logic)
		{
			logic.OnClausesGroup = InvalidateOneClauseGroup(gkBase, logic.OnClausesGroup);
			logic.OffClausesGroup = InvalidateOneClauseGroup(gkBase, logic.OffClausesGroup);
			logic.StopClausesGroup = InvalidateOneClauseGroup(gkBase, logic.StopClausesGroup);
			logic.OnNowClausesGroup = InvalidateOneClauseGroup(gkBase, logic.OnNowClausesGroup);
			logic.OffNowClausesGroup = InvalidateOneClauseGroup(gkBase, logic.OffNowClausesGroup);
		}

	   public GKClauseGroup InvalidateOneClauseGroup(GKBase gkBase, GKClauseGroup clauseGroup)
		{
			var result = new GKClauseGroup();
			result.ClauseJounOperationType = clauseGroup.ClauseJounOperationType;
			var groups = new List<GKClauseGroup>();
			foreach (var group in clauseGroup.ClauseGroups)
			{
				var _clauseGroup = InvalidateOneClauseGroup(gkBase, group);
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
				clause.Doors = new List<GKDoor>();
				clause.PumpStations = new List<GKPumpStation>();

				var deviceUIDs = new List<Guid>();
				foreach (var deviceUID in clause.DeviceUIDs)
				{
					var device = Devices.FirstOrDefault(x => x.UID == deviceUID);
					if (device != null)
					{
						deviceUIDs.Add(deviceUID);
						clause.Devices.Add(device);
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
					}
				}
				clause.DelayUIDs = delayUIDs;

				var doorUIDs = new List<Guid>();
				foreach (var doorUID in clause.DoorUIDs)
				{
					var door = Doors.FirstOrDefault(x => x.UID == doorUID);
					if (door != null)
					{
						doorUIDs.Add(doorUID);
						clause.Doors.Add(door);
					}
				}
				clause.DoorUIDs = doorUIDs;

				var pumpStationsUIDs = new List<Guid>();
				foreach (var pumpStationUID in clause.PumpStationsUIDs)
				{
					var pumpStation = PumpStations.FirstOrDefault(x => x.UID == pumpStationUID);
					if (pumpStation != null)
					{
						pumpStationsUIDs.Add(pumpStationUID);
						clause.PumpStations.Add(pumpStation);
					}
				}
				clause.PumpStationsUIDs = pumpStationsUIDs;

				if (clause.HasObjects())
					result.Clauses.Add(clause);
			}
			return result;
		}

		public void InvalidateGKCodeReaderSettingsPart(GKCodeReaderSettingsPart codeReaderSettingsPart)
		{
			var codeUIDs = new List<Guid>();
			foreach (var codeUID in codeReaderSettingsPart.CodeUIDs)
			{
				var code = Codes.FirstOrDefault(x => x.UID == codeUID);
				if (code != null)
				{
					codeUIDs.Add(codeUID);
				}
			}
			codeReaderSettingsPart.CodeUIDs = codeUIDs;
		}

		void InitializeCodes()
		{
			foreach (var code in Codes)
			{
			}
		}

		void InitializeOPC()
		{
			OPCSettings.ZoneUIDs = Zones.Where(x => OPCSettings.ZoneUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DelayUIDs = Delays.Where(x => OPCSettings.DelayUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DeviceUIDs = Devices.Where(x => OPCSettings.DeviceUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DiretionUIDs = Directions.Where(x => OPCSettings.DiretionUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.GuardZoneUIDs = GuardZones.Where(x => OPCSettings.GuardZoneUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.MPTUIDs = MPTs.Where(x => OPCSettings.MPTUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.NSUIDs = PumpStations.Where(x => OPCSettings.NSUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
			OPCSettings.DoorUIDs = Doors.Where(x => OPCSettings.DoorUIDs.Contains(x.UID)).Select(x => x.UID).ToList();
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
			if (device.DriverType == GKDriverType.GK || device.DriverType == GKDriverType.GKMirror)
			{
				var indicatorsGroupDevice = device.Children.FirstOrDefault(x => x.DriverType == GKDriverType.GKIndicatorsGroup);
				var relaysGroupDevice = device.Children.FirstOrDefault(x => x.DriverType == GKDriverType.GKRelaysGroup);

				if (indicatorsGroupDevice != null && indicatorsGroupDevice.Children.Count == 16)
				{
					indicatorsGroupDevice.Children[0].PredefinedName = "Неисправность";
					indicatorsGroupDevice.Children[1].PredefinedName = "Пожар 1";
					indicatorsGroupDevice.Children[2].PredefinedName = "Пожар 2";
					indicatorsGroupDevice.Children[3].PredefinedName = "Внимание";
					indicatorsGroupDevice.Children[4].PredefinedName = "Включение ПУСК";
					indicatorsGroupDevice.Children[5].PredefinedName = "Тест";
					indicatorsGroupDevice.Children[6].PredefinedName = "Отключение";
					indicatorsGroupDevice.Children[7].PredefinedName = "Автоматика отключена";
					indicatorsGroupDevice.Children[8].PredefinedName = "Звук отключен";
					indicatorsGroupDevice.Children[9].PredefinedName = "Останов пуска";
					indicatorsGroupDevice.Children[10].PredefinedName = "Тревога";
					indicatorsGroupDevice.Children[11].PredefinedName = "Резерв 1";
					indicatorsGroupDevice.Children[12].PredefinedName = "Резерв 2";
					indicatorsGroupDevice.Children[13].PredefinedName = "Резерв 3";
					indicatorsGroupDevice.Children[14].PredefinedName = "Резерв 4";
					indicatorsGroupDevice.Children[15].PredefinedName = "Резерв 5";
				}
				if (relaysGroupDevice != null && relaysGroupDevice.Children.Count == 5)
				{
					relaysGroupDevice.Children[0].PredefinedName = "Реле 1";
					relaysGroupDevice.Children[1].PredefinedName = "Реле 2";
					relaysGroupDevice.Children[2].PredefinedName = "Реле 3";
					relaysGroupDevice.Children[3].PredefinedName = "Реле 4";
					relaysGroupDevice.Children[4].PredefinedName = "Реле 5";
				}
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
				GKManager.SetDeviceLogic(mptDevice.Device, new GKLogic());
				mptDevice.Device.ZoneUIDs = new List<Guid>();
				mptDevice.Device.Zones.Clear();
			}
		}

		public void SetMPTDefaultProperty(GKDevice device, GKMPTDeviceType mptDeviceType)
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
					case GKDriverType.RSR2_SCOPA:
					case GKDriverType.RSR2_ZOV:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Время удержания, с", 0);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние для модуля Выключено", 0);
						SetDeviceProperty(device, "Состояние для режима Удержания", 4);
						if (mptDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDeviceType == GKMPTDeviceType.ExitBoard)
						{
							SetDeviceProperty(device, "Состояние для режима Включено", 32);
						}
						if (mptDeviceType == GKMPTDeviceType.Speaker || mptDeviceType == GKMPTDeviceType.AutomaticOffBoard)
						{
							SetDeviceProperty(device, "Состояние для режима Включено", 16);
						}
						break;

					case GKDriverType.RSR2_MVK8:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Удержания", 4);
						SetDeviceProperty(device, "Контроль", 3);
						SetDeviceProperty(device, "Норма питания, 0.1В", 80);
						if (mptDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDeviceType == GKMPTDeviceType.ExitBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 32);
						}

						if (mptDeviceType == GKMPTDeviceType.Bomb)
						{
							SetDeviceProperty(device, "Время удержания, с", 2);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
						}

						if (mptDeviceType == GKMPTDeviceType.Speaker || mptDeviceType == GKMPTDeviceType.AutomaticOffBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 16);
						}
						break;

					case GKDriverType.RSR2_RM_1:
						SetDeviceProperty(device, "Задержка на включение, с", 0);
						SetDeviceProperty(device, "Задержка на выключение, с", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Выключено", 0);
						SetDeviceProperty(device, "Состояние контакта для режима Удержания", 4);
						if (mptDeviceType == GKMPTDeviceType.DoNotEnterBoard || mptDeviceType == GKMPTDeviceType.ExitBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 32);
						}
						if (mptDeviceType == GKMPTDeviceType.Bomb)
						{
							SetDeviceProperty(device, "Время удержания, с", 2);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 0);
						}
						if (mptDeviceType == GKMPTDeviceType.Speaker || mptDeviceType == GKMPTDeviceType.AutomaticOffBoard)
						{
							SetDeviceProperty(device, "Время удержания, с", 0);
							SetDeviceProperty(device, "Состояние контакта для режима Включено", 16);
						}
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