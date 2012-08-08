using System;
using System.Collections.Generic;
using System.Linq;
using AlarmModule.Events;
using Common;
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

		void OnDeviceStateChanged(Guid obj)
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
			foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
			{
				foreach (var state in deviceState.States)
				{
					if (deviceState.Device == null) continue;

					AlarmType? alarmType = StateToAlarmType(state, deviceState.Device.Driver);
					if (alarmType.HasValue == false)
						continue;

					var stateType = state.DriverState.StateType;

					var alarm = Alarms.FirstOrDefault(x => ((x.StateType == stateType) && (x.DeviceUID == deviceState.UID)));
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
							DeviceUID = deviceState.UID,
							ZoneNo = deviceState.Device.ZoneNo,
							Time = state.Time,
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
			foreach (var zoneState in FiresecManager.DeviceStates.ZoneStates)
			{
				switch (zoneState.Zone.ZoneType)
				{
					case ZoneType.Fire:
						if (zoneState.StateType == StateType.Fire)
						{
							var alarm = Alarms.FirstOrDefault(x => ((x.StateType == StateType.Fire) && (x.ZoneNo == zoneState.No)));
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
									ZoneNo = zoneState.No,
									Time = DateTime.Now,
									StateName = "Пожар"
								};
								Alarms.Add(newAlarm);
								ServiceFactory.Events.GetEvent<AlarmAddedEvent>().Publish(newAlarm);
							}
						}
						break;
					case ZoneType.Guard:
						if (FiresecManager.IsZoneOnGuardAlarm(zoneState))
						{
							var guardAlarm = Alarms.FirstOrDefault(x => ((x.AlarmType == AlarmType.Guard) && (x.ZoneNo == zoneState.No)));
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
									ZoneNo = zoneState.No,
									Time = DateTime.Now,
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
			foreach (var device in FiresecManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType != DriverType.Valve)
					continue;

				var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
				if (deviceState == null)
					continue;

				foreach (var state in deviceState.States)
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

			if (state.DriverState.IsAutomatic && (state.DriverState.Code.Contains("AutoOff") || state.DriverState.Code.Contains("Auto_Off")))
				alarmType = AlarmType.Auto;

			return alarmType;
		}
	}
}