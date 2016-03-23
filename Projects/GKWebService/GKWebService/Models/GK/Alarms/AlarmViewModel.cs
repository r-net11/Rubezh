using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Controls.Converters;
using Infrastructure.Common.Windows;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;
using Infrustructure.Plans.Interfaces;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmViewModel
	{
		private Alarm Alarm { get; set; }

		public GKAlarmType AlarmType { get; set; }

		public GKBaseModel GkEntity { get; set; }

		public string AlarmImageSource { get; set; }

		public string AlarmName { get; set; }

		public string AlarmColor { get; set; }

		public bool CanReset { get; set; }

		public bool CanResetIgnore { get; set; }

		public bool CanTurnOnAutomatic { get; set; }

		public bool CanShowProperties { get; set; }

		public List<PlanLinkViewModel> Plans { get; set; }

		public AlarmViewModel()
		{
		}

		public AlarmViewModel(Alarm alarm)
		{
			Alarm = alarm;
			AlarmType = alarm.AlarmType;
            AlarmImageSource = ((string)new AlarmTypeToBIconConverter().Convert(alarm.AlarmType, null, null, null)).Substring(36).Replace(".png", "");
			AlarmName = alarm.AlarmType.ToDescription();
			GkEntity = GetGkEntity();
			CanReset = GetCanReset();
			CanResetIgnore = GetCanResetIgnore();
			CanTurnOnAutomatic = GetCanTurnOnAutomatic();
			CanShowProperties = GetCanShowProperties();
			AlarmColor = new AlarmTypeToColorConverter().Convert(alarm.AlarmType);
			InitializePlans();
		}

		private GKBaseModel GetGkEntity()
		{
			GKDevice device;
			GKZone zone;
			GKGuardZone guardZone;
			GKDirection direction;
			GKPumpStation pumpStation;
			GKMPT mpt;
			GKDelay delay;
			GKDoor door;

			if ((device = Alarm.GkBaseEntity as GKDevice) != null)
			{
				return new Device(device);
			}
			if ((zone = Alarm.GkBaseEntity as GKZone) != null)
			{
				return new FireZone.FireZone(zone);
			}
			if ((guardZone = Alarm.GkBaseEntity as GKGuardZone) != null)
			{
				return new GuardZones.GuardZone(guardZone);
			}
			if ((direction = Alarm.GkBaseEntity as GKDirection) != null)
			{
				return new Direction(direction);
			}
			if ((pumpStation = Alarm.GkBaseEntity as GKPumpStation) != null)
			{
				return new PumpStation.PumpStation(pumpStation);
			}
			if ((mpt = Alarm.GkBaseEntity as GKMPT) != null)
			{
				return new MPTModel(mpt);
			}
			if ((delay = Alarm.GkBaseEntity as GKDelay) != null)
			{
				return new Delay(delay);
			}
			if ((door = Alarm.GkBaseEntity as GKDoor) != null)
			{
				return new Door.Door(door);
			}

			throw new InvalidOperationException("Не найден GkBaseEntity");
		}

		public void Reset()
		{
			if (GkEntity.ObjectType == GKBaseObjectType.Zone)
			{
				var zone = new GKZone {UID = GkEntity.UID};
				switch (AlarmType)
				{
					case GKAlarmType.Fire1:
						ClientManager.FiresecService.GKResetFire1(zone);
						break;

					case GKAlarmType.Fire2:
						ClientManager.FiresecService.GKResetFire2(zone);
						break;
				}
			}

			if (GkEntity.ObjectType == GKBaseObjectType.GuardZone)
			{
				var guardZone = new GKGuardZone { UID = GkEntity.UID };
				switch (AlarmType)
				{
					case GKAlarmType.GuardAlarm:
						ClientManager.FiresecService.GKReset(guardZone);
						break;
				}
			}

			if (GkEntity.ObjectType == GKBaseObjectType.Device)
			{
				var device = new GKDevice { UID = GkEntity.UID };
				ClientManager.FiresecService.GKReset(device);
			}

			if (GkEntity.ObjectType == GKBaseObjectType.Door)
			{
				var door = new GKDoor { UID = GkEntity.UID };
				switch (AlarmType)
				{
					case GKAlarmType.GuardAlarm:
						ClientManager.FiresecService.GKReset(door);
						break;
				}
			}
		}

		public void ResetIgnore()
		{
			var alarms = AlarmsViewModel.OnGKObjectsStateChanged(null);
			var alarm = alarms.FirstOrDefault(a => a.GkBaseEntity.UID == GkEntity.UID);
			if (alarm != null)
			{
				if (alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(alarm.GkBaseEntity);
				}
			}
		}

		public void TurnOnAutomatic()
		{
			var alarms = AlarmsViewModel.OnGKObjectsStateChanged(null);
			var alarm = alarms.FirstOrDefault(a => a.GkBaseEntity.UID == GkEntity.UID);

			if (alarm == null)
			{
				return;
			}

			var device = alarm.GkBaseEntity as GKDevice;
			if (device != null)
			{
				if (device.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(device);
				}
			}

			var direction = alarm.GkBaseEntity as GKDirection;
			if (direction != null)
			{
				if (direction.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(direction);
				}
			}

			var pumpStation = alarm.GkBaseEntity as GKPumpStation;
			if (pumpStation != null)
			{
				if (pumpStation.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_NS_Control))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(pumpStation);
				}
			}

			var delay = alarm.GkBaseEntity as GKDelay;
			if (delay != null)
			{
				if (delay.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(delay);
				}
			}

			var mpt = alarm.GkBaseEntity as GKMPT;
			if (mpt != null)
			{
				if (mpt.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(mpt);
				}
			}

			var guardZone = alarm.GkBaseEntity as GKGuardZone;
			if (guardZone != null)
			{
				if (guardZone.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(guardZone);
				}
			}

			var door = alarm.GkBaseEntity as GKDoor;
			if (door != null)
			{
				if (door.State.StateClasses.Contains(XStateClass.AutoOff) && ClientManager.CheckPermission(PermissionType.Oper_Door_Control))
				{
					ClientManager.FiresecService.GKSetAutomaticRegime(door);
				}
			}
		}

		bool GetCanReset()
		{
			if (Alarm.GkBaseEntity as GKZone != null)
			{
				return (Alarm.AlarmType == GKAlarmType.Fire1 || Alarm.AlarmType == GKAlarmType.Fire2);
			}
			if (Alarm.GkBaseEntity as GKGuardZone != null)
			{
				return (Alarm.AlarmType == GKAlarmType.GuardAlarm);
			}

			var device = Alarm.GkBaseEntity as GKDevice;
			if (device != null)
			{
				if (device.DriverType == GKDriverType.RSR2_MAP4)
				{
					return device.State.StateClasses.Contains(XStateClass.Fire2) || device.State.StateClasses.Contains(XStateClass.Fire1);
				}
			}
			if (Alarm.GkBaseEntity as GKDoor != null)
			{
				return (Alarm.AlarmType == GKAlarmType.GuardAlarm);
			}
			return false;
		}

		bool GetCanResetIgnore()
		{
			if (Alarm.AlarmType != GKAlarmType.Ignore)
				return false;
			if (Alarm.GkBaseEntity is GKDevice)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.GkBaseEntity is GKZone)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.GkBaseEntity is GKGuardZone)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.GkBaseEntity is GKMPT)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDelay)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDirection)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.GkBaseEntity is GKPumpStation)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDoor)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore))
					return true;
			}
			return false;
		}

		bool GetCanTurnOnAutomatic()
		{
			if (Alarm.AlarmType == GKAlarmType.AutoOff)
			{
				if (Alarm.GkBaseEntity != null)
				{
					var device = Alarm.GkBaseEntity as GKDevice;
					if (device != null)
					{
						return device.Driver.IsControlDevice && Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.AutoOff);
					}
					return Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.AutoOff);
				}
			}
			return false;
		}

		bool GetCanShowProperties()
		{
			return Alarm.GkBaseEntity != null;
		}

		void InitializePlans()
		{
			Plans = new List<PlanLinkViewModel>();

			foreach (var plan in RubezhClient.ClientManager.PlansConfiguration.AllPlans)
			{
				var elementUnion = plan.ElementUnion;
				var gkBaseEntity = Alarm.GkBaseEntity as IPlanPresentable;
				if (gkBaseEntity != null)
				{
					foreach (var planElementUID in gkBaseEntity.PlanElementUIDs)
					{
						var elementBase = elementUnion.FirstOrDefault(x => x.UID == planElementUID);
						if (elementBase != null)
						{
							var alarmPlanViewModel = new PlanLinkViewModel(plan, elementBase);
							alarmPlanViewModel.GkBaseEntityUID = Alarm.GkBaseEntity.UID;
							Plans.Add(alarmPlanViewModel);
							break;
						}
					}
				}
			}
		}
	}
}