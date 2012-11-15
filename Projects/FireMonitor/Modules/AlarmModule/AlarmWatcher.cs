using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure;
using Infrastructure.Events;
using AlarmModule.ViewModels;

namespace AlarmModule
{
    public class AlarmWatcher
    {
        List<Alarm> Alarms;

        public AlarmWatcher()
        {
            Alarms = new List<Alarm>();
            ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Unsubscribe(OnDevicesStateChanged);
            ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Subscribe(OnDevicesStateChanged);
            OnDevicesStateChanged(null);
        }

        void OnDevicesStateChanged(object obj)
        {
            Alarms = new List<Alarm>();
			UpdateZoneAlarms();
            UpdateDeviceAlarms();
            AlarmsViewModel.Current.Update(Alarms);
            AlarmGroupsViewModel.Current.Update(Alarms);
        }

        void UpdateDeviceAlarms()
        {
            foreach (var device in FiresecManager.Devices)
            {
                foreach (var state in device.DeviceState.ThreadSafeStates)
                {
                    AlarmType? alarmType = StateToAlarmType(state, device.Driver);
                    if (alarmType.HasValue == false)
                        continue;

                    var newAlarm = new Alarm()
                    {
                        AlarmType = alarmType.Value,
                        StateType = state.DriverState.StateType,
                        Device = device,
                        Zone = device.Zone,
                        StateName = state.DriverState.Name
                    };
                    Alarms.Add(newAlarm);
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
                            var newAlarm = new Alarm()
                            {
                                AlarmType = AlarmType.Fire,
                                StateType = StateType.Fire,
                                Zone = zone,
                                StateName = "Пожар"
                            };
                            Alarms.Add(newAlarm);
                        }
                        break;
                    case ZoneType.Guard:
                        if (FiresecManager.IsZoneOnGuardAlarm(zone.ZoneState))
                        {
                            var newAlarm = new Alarm()
                            {
                                AlarmType = AlarmType.Guard,
                                StateType = StateType.Fire,
                                Zone = zone,
                                StateName = "Тревога"
                            };
                            Alarms.Add(newAlarm);
                        }
                        break;
                }
            }
        }

        AlarmType? StateToAlarmType(DeviceDriverState state, Driver driver)
        {
            if (state.DriverState.IsAutomatic && (state.DriverState.Code.Contains("AutoOff") || state.DriverState.Code.Contains("Auto_Off") || state.DriverState.Code.Contains("Auto_off")))
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