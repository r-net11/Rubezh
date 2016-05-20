using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmsViewModel
	{
		public List<AlarmViewModel> Alarms { get; set; }

		public bool CanResetAll { get; set; }

		public bool CanResetIgnoreAll { get; set; }

		public AlarmsViewModel()
		{
			Alarms = new List<AlarmViewModel>();
		}

		public static List<Alarm> OnGKObjectsStateChanged(object obj)
		{
			var alarms = new List<Alarm>();
			foreach (var device in GKManager.Devices)
			{
				if (!device.IsRealDevice)
					continue;

				foreach (var stateClass in device.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, device));
							break;

						case XStateClass.Failure:
							alarms.Add(new Alarm(GKAlarmType.Failure, device));
							break;

						case XStateClass.On:
						case XStateClass.TurningOn:
							if (device.Driver.IsControlDevice)
							{
								if (!alarms.Any(x => x.AlarmType == GKAlarmType.Turning && x.GkBaseEntity.UID == device.UID))
								{
									alarms.Add(new Alarm(GKAlarmType.Turning, device));
								}
							}
							break;
					}
				}
				if (device.State.StateClasses.Contains(XStateClass.AutoOff) && device.Driver.IsControlDevice)
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, device));
				}
				if (device.State.StateClasses.Contains(XStateClass.Service))
				{
					alarms.Add(new Alarm(GKAlarmType.Service, device));
				}
			}

			foreach (var zone in GKManager.Zones)
			{
				foreach (var stateClass in zone.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Fire2:
							alarms.Add(new Alarm(GKAlarmType.Fire2, zone));
							break;

						case XStateClass.Fire1:
							alarms.Add(new Alarm(GKAlarmType.Fire1, zone));
							break;

						case XStateClass.Attention:
							alarms.Add(new Alarm(GKAlarmType.Attention, zone));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, zone));
							break;
					}
				}
			}

			foreach (var guardZone in GKManager.GuardZones)
			{
				foreach (var stateClass in guardZone.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Fire1:
							alarms.Add(new Alarm(GKAlarmType.GuardAlarm, guardZone));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, guardZone));
							break;

						case XStateClass.Attention:
							alarms.Add(new Alarm(GKAlarmType.Attention, guardZone));
							break;

						case XStateClass.AutoOff:
							alarms.Add(new Alarm(GKAlarmType.AutoOff, guardZone));
							break;
					}
				}
			}

			foreach (var door in GKManager.Doors)
			{
				foreach (var stateClass in door.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.Fire2:
							alarms.Add(new Alarm(GKAlarmType.Fire2, door));
							break;

						case XStateClass.Fire1:
							alarms.Add(new Alarm(GKAlarmType.GuardAlarm, door));
							break;

						case XStateClass.Attention:
							alarms.Add(new Alarm(GKAlarmType.Attention, door));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, door));
							break;

						case XStateClass.AutoOff:
							alarms.Add(new Alarm(GKAlarmType.AutoOff, door));
							break;
					}
				}
			}

			foreach (var direction in GKManager.Directions)
			{
				foreach (var stateClass in direction.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.NPTOn, direction));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, direction));
							break;
					}
				}
				if (direction.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, direction));
				}
			}

			foreach (var pumpStation in GKManager.PumpStations)
			{
				foreach (var stateClass in pumpStation.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.NPTOn, pumpStation));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, pumpStation));
							break;
					}
				}
				if (pumpStation.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, pumpStation));
				}
			}

			foreach (var mpt in GKManager.MPTs)
			{
				foreach (var stateClass in mpt.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.NPTOn, mpt));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, mpt));
							break;
					}
				}
				if (mpt.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, mpt));
				}
			}

			foreach (var delay in GKManager.Delays)
			{
				foreach (var stateClass in delay.State.StateClasses)
				{
					switch (stateClass)
					{
						case XStateClass.On:
						case XStateClass.TurningOn:
							alarms.Add(new Alarm(GKAlarmType.Turning, delay));
							break;

						case XStateClass.Ignore:
							alarms.Add(new Alarm(GKAlarmType.Ignore, delay));
							break;
					}
				}
				if (delay.State.StateClasses.Contains(XStateClass.AutoOff))
				{
					alarms.Add(new Alarm(GKAlarmType.AutoOff, delay));
				}
			}

			alarms = (from Alarm alarm in alarms orderby alarm.AlarmType select alarm).ToList();

			return alarms;
		}

		public static int GetAlarmsToResetCount()
		{
			int result = 0;
			foreach (var zone in GKManager.Zones)
			{
				if (zone.State.StateClasses.Contains(XStateClass.Fire1))
					result++;
				if (zone.State.StateClasses.Contains(XStateClass.Fire2))
					result++;
			}
			foreach (var zone in GKManager.GuardZones)
			{
				if (zone.State.StateClasses.Contains(XStateClass.Fire1))
					result++;
			}
			foreach (var device in GKManager.Devices)
			{
				if (device.DriverType == GKDriverType.RSR2_MAP4)
				{
					if (device.State.StateClasses.Contains(XStateClass.Fire1) || device.State.StateClasses.Contains(XStateClass.Fire2))
						result++;
				}
			}
			foreach (var door in GKManager.Doors)
			{
				if (door.State.StateClasses.Contains(XStateClass.Fire1))
					result++;
			}
			return result;
		}

		public void UpdateAlarms(List<Alarm> alarms)
		{
			CanResetAll = (GetAlarmsToResetCount() > 0);
			CanResetIgnoreAll = GetCanResetIgnoreAll();

			foreach (var alarm in alarms)
			{
				var alarmViewModel = new AlarmViewModel(alarm);
				Alarms.Add(alarmViewModel);
			}
		}

		public static void ResetAll()
		{
			foreach (var zone in GKManager.Zones)
			{
				if (zone.State.StateClasses.Contains(XStateClass.Fire1))
				{
					ClientManager.RubezhService.GKResetFire1(zone);
				}
				if (zone.State.StateClasses.Contains(XStateClass.Fire2))
				{
					ClientManager.RubezhService.GKResetFire2(zone);
				}
			}
			foreach (var guardZone in GKManager.GuardZones)
			{
				if (guardZone.State.StateClasses.Contains(XStateClass.Fire1))
				{
					ClientManager.RubezhService.GKReset(guardZone);
				}
			}
			foreach (var door in GKManager.Doors)
			{
				if (door.State.StateClasses.Contains(XStateClass.Fire1))
				{
					ClientManager.RubezhService.GKReset(door);
				}
			}
		}

		public static void ResetIgnoreAll()
		{
			foreach (var device in GKManager.Devices)
			{
				if (!device.Driver.IsDeviceOnShleif)
					continue;

				if (device.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(device);
				}
			}

			foreach (var zone in GKManager.Zones)
			{
				if (zone.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(zone);
				}
			}

			foreach (var guardZones in GKManager.GuardZones)
			{
				if (guardZones.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(guardZones);
				}
			}

			foreach (var door in GKManager.Doors)
			{
				if (door.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Door_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(door);
				}
			}

			foreach (var direction in GKManager.Directions)
			{
				if (direction.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(direction);
				}
			}

			foreach (var pumpStation in GKManager.PumpStations)
			{
				if (pumpStation.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_NS_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(pumpStation);
				}
			}

			foreach (var mpt in GKManager.MPTs)
			{
				if (mpt.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(mpt);
				}
			}

			foreach (var delay in GKManager.Delays)
			{
				if (delay.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
				{
					ClientManager.RubezhService.GKSetAutomaticRegime(delay);
				}
			}
		}

		public static bool GetCanResetIgnoreAll()
		{
			try
			{
				foreach (var device in GKManager.Devices)
				{
					if (!device.Driver.IsDeviceOnShleif)
						continue;

					if (device.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var zone in GKManager.Zones)
				{
					if (zone.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var guardZone in GKManager.GuardZones)
				{
					if (guardZone.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var door in GKManager.Doors)
				{
					if (door.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var direction in GKManager.Directions)
				{
					if (direction.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var pumpStation in GKManager.PumpStations)
				{
					if (pumpStation.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var mpt in GKManager.MPTs)
				{
					if (mpt.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}

				foreach (var delay in GKManager.Delays)
				{
					if (delay.State.StateClasses.Contains(XStateClass.Ignore))
						return true;
				}
				return false;
			}
			catch
			{
				return false;
			}
		}
	}
}