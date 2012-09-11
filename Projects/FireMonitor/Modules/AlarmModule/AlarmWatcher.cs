using System;
using System.Collections.Generic;
using System.Linq;
using AlarmModule.Events;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;

namespace AlarmModule
{
	public class AlarmWatcher
	{
		List<Alarm> Alarms;

		public AlarmWatcher()
		{
			Alarms = new List<Alarm>();
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Unsubscribe(OnDeviceStateChanged);
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Subscribe(OnDeviceStateChanged);
			OnDeviceStateChanged(Guid.Empty);
		}

		void OnDeviceStateChanged(Guid deviceUID)
		{
			Alarms.ForEach(x => x.IsDeleting = true);

			UpdateDeviceAlarms();
			UpdateZoneAlarms();

			if (ServiceFactory.AppSettings.CanControl)
				UpdateValveTimer();

			foreach (var alarm in Alarms)
			{
				if (alarm.IsDeleting)
					ServiceFactory.Events.GetEvent<AlarmRemovedEvent>().Publish(alarm);
			}

			Alarms.RemoveAll(x => x.IsDeleting);
		}

		void UpdateDeviceAlarms()
		{
			foreach (var device in FiresecManager.Devices)
			{
				foreach (var state in device.DeviceState.States)
				{
					AlarmType? alarmType = StateToAlarmType(state, device.Driver);
					if (alarmType.HasValue == false)
						continue;

					var stateType = state.DriverState.StateType;

					var alarm = Alarms.FirstOrDefault(x => ((x.StateType == stateType) && (x.DeviceUID == device.UID)));
					if (alarm != null)
					{
						alarm.IsDeleting = false;
					}
					else
					{
						var newAlarm = new Alarm()
						{
							AlarmType = alarmType.Value,
							StateType = stateType,
							DeviceUID = device.UID,
							ZoneNo = device.ZoneNo,
							StateName = state.DriverState.Name
						};
						Alarms.Add(newAlarm);
						ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(newAlarm);
					}
				}
			}
		}

		void UpdateZoneAlarms()
		{
			foreach (var zone in FiresecManager.Zones)
			{
				switch (zone.ZoneType)
				{
					case ZoneType.Fire:
						if (zone.ZoneState.StateType == StateType.Fire)
						{
							var alarm = Alarms.FirstOrDefault(x => ((x.StateType == StateType.Fire) && (x.ZoneNo == zone.ZoneState.No)));
							if (alarm != null)
							{
								alarm.IsDeleting = false;
							}
							else
							{
								var newAlarm = new Alarm()
								{
									AlarmType = AlarmType.Fire,
									StateType = StateType.Fire,
									ZoneNo = zone.No,
									StateName = "Пожар"
								};
								Alarms.Add(newAlarm);
								ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(newAlarm);
							}
						}
						break;
					case ZoneType.Guard:
						if (FiresecManager.IsZoneOnGuardAlarm(zone.ZoneState))
						{
							var guardAlarm = Alarms.FirstOrDefault(x => ((x.AlarmType == AlarmType.Guard) && (x.ZoneNo == zone.No)));
							if (guardAlarm != null)
							{
								guardAlarm.IsDeleting = false;
							}
							else
							{
								var newAlarm = new Alarm()
								{
									AlarmType = AlarmType.Guard,
									StateType = StateType.Fire,
									ZoneNo = zone.No,
									StateName = "Тревога"
								};
								Alarms.Add(newAlarm);
								ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(newAlarm);
							}
						}
						break;
				}
			}
		}

		void UpdateValveTimer()
		{
			foreach (var device in FiresecManager.Devices)
			{
				if (device.Driver.DriverType != DriverType.Valve)
					continue;

				foreach (var state in device.DeviceState.States)
				{
					if (state.Code == "Bolt_On")
					{
						if (state.Time.HasValue == false)
							continue;

						var timeoutProperty = device.Properties.FirstOrDefault(x => x.Name == "Timeout");
						if (timeoutProperty == null)
							continue;

						int delta = 0;
						try
						{
							var timeSpan = DateTime.Now - state.Time.Value;
							delta = int.Parse(timeoutProperty.Value) - timeSpan.Seconds;
						}
						catch (Exception e)
						{
							Logger.Error(e, "Исключение при вызове AlarmWatcher.UpdateValveTimer");
							continue;
						}

						if (delta > 0)
							ServiceFactory.Events.GetEvent<ShowDeviceDetailsEvent>().Publish(device.UID);
					}
				}
			}
		}

		AlarmType? StateToAlarmType(DeviceDriverState state, Driver driver)
		{
			if (state.DriverState.IsAutomatic && (state.DriverState.Code.Contains("AutoOff") || state.DriverState.Code.Contains("Auto_Off")))
				return AlarmType.Auto;

			AlarmType? alarmType = null;
			switch (state.DriverState.StateType)
			{
				case StateType.Fire:
					return null;

				case StateType.Attention:
					if (state.DriverState.CanResetOnPanel == false)
						return null;
					alarmType = AlarmType.Attention;
					break;

				case StateType.Info:
					if (state.DriverState.CanResetOnPanel == false)
						return null;
					alarmType = AlarmType.Info;
					break;

				case StateType.Off:
					if (driver.CanDisable == false)
						return null;
					alarmType = AlarmType.Off;
					break;

				case StateType.Failure:
					alarmType = AlarmType.Failure;
					break;

				case StateType.Service:
					alarmType = AlarmType.Service;
					break;
			}

			return alarmType;
		}
	}
}