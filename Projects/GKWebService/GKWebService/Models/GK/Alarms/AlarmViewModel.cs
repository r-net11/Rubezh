using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Controls.Converters;
using RubezhAPI;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhClient;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmViewModel
	{
		private Alarm Alarm { get; set; }

		public string AlarmImageSource { get; set; }

		public string AlarmName { get; set; }

		public string ObjectName { get; set; }

		public string ObjectImageSource { get; set; }

		public string ObjectStateClass { get; set; }

		public bool CanReset { get; set; }

		public bool CanResetIgnore { get; set; }

		public bool CanTurnOnAutomatic { get; set; }

		public bool CanShowProperties { get; set; }

		public AlarmViewModel(Alarm alarm)
		{
			Alarm = alarm;
			AlarmImageSource = ((string)new AlarmTypeToBIconConverter().Convert(alarm.AlarmType, null, null, null)).Substring(36).Replace(".png", "");
			AlarmName = alarm.AlarmType.ToDescription();
            ObjectName = alarm.GkBaseEntity.PresentationName;
			ObjectImageSource = alarm.GkBaseEntity.ImageSource.Substring(20).Replace(".png", "");
			ObjectStateClass = alarm.GkBaseEntity.State.StateClass.ToString();
			CanReset = GetCanReset();
			CanResetIgnore = GetCanResetIgnore();
			CanTurnOnAutomatic = GetCanTurnOnAutomatic();
			CanShowProperties = GetCanShowProperties();
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
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Device_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKZone)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Zone_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKGuardZone)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_GuardZone_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKMPT)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_MPT_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDelay)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Delay_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKDirection)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_Directions_Control))
					return true;
			}

			if (Alarm.GkBaseEntity is GKPumpStation)
			{
				if (Alarm.GkBaseEntity.State.StateClasses.Contains(XStateClass.Ignore) && ClientManager.CheckPermission(PermissionType.Oper_NS_Control))
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
	}
}