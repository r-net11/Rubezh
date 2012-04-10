﻿using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using FiresecClient;

namespace AlarmModule
{
    public class Alarm
    {
        public AlarmType AlarmType { get; set; }
        public StateType StateType { get; set; }
        public Guid DeviceUID { get; set; }
        public ulong? ZoneNo { get; set; }
        public DateTime? Time { get; set; }
        public string StateName { get; set; }
        public bool IsDeleting { get; set; }

        public AlarmEntityType AlarmEntityType
        {
            get
            {
                if (AlarmType == AlarmType.Fire)
                    return AlarmEntityType.Zone;
                return AlarmEntityType.Device;
            }
        }

        public bool IsConfirmed { get; private set; }

        public void Confirm()
        {
            IsConfirmed = true;

            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == ZoneNo);

            FiresecManager.AddJournalRecord(new JournalRecord()
            {
                SystemTime = DateTime.Now,
                DeviceTime = DateTime.Now,
                ZoneName = zone.No.ToString(),
                Description = "Состояние \"" + StateName + "\" подтверждено оператором",
                StateType = StateType.Info
            });
        }

        public bool CanRemoveFromIgnoreList()
        {
            return ((StateType == StateType.Off) &&
            (FiresecManager.CurrentUser.Permissions.Any(x => x == PermissionType.Oper_RemoveFromIgnoreList)));
        }

        public void RemoveFromIgnoreList()
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == DeviceUID);
            if (deviceState.CanDisable() && deviceState.IsDisabled)
                FiresecManager.RemoveFromIgnoreList(new List<Guid>() { deviceState.Device.UID });
        }

        public bool CanReset()
        {
            return StateType != StateType.Off;
        }

        public void Reset()
        {
            var resetItems = new List<ResetItem>();
            resetItems.Add(GetResetItem());
            FiresecManager.ResetStates(resetItems);
        }

        public ResetItem GetResetItem()
        {
            switch (AlarmEntityType)
            {
                case AlarmEntityType.Device:
                    return GetDeviceResetItem();

                case AlarmEntityType.Zone:
                    return GetZoneResetItem();

                default:
                    return null;
            }
        }

        ResetItem GetZoneResetItem()
        {
            foreach (var device in FiresecManager.DeviceConfiguration.Devices)
            {
                if (device.ZoneNo == ZoneNo)
                {
                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
                    if (deviceState.States.Any(x => x.DriverState.StateType == StateType.Fire))
                    {
                        DeviceUID = deviceState.UID;
                        return GetDeviceResetItem();
                    }
                }
            }

            return null;
        }

        ResetItem GetDeviceResetItem()
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == DeviceUID);
            var parentDevice = device.Parent;
            var parentDeviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == parentDevice.UID);

            var resetItem = new ResetItem();

            switch (AlarmType)
            {
                case AlarmType.Fire:
                case AlarmType.Attention:
                case AlarmType.Info:
                case AlarmType.Failure:
                    resetItem.DeviceUID = parentDeviceState.UID;
                    foreach (var state in parentDeviceState.States)
                    {
                        if (state.DriverState.StateType == EnumsConverter.AlarmTypeToStateType(AlarmType) && state.DriverState.IsManualReset)
                            resetItem.StateNames.Add(state.DriverState.Name);
                    }
                    break;

                case AlarmType.Auto:
                    resetItem.DeviceUID = device.UID;
                    var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == device.UID);
                    foreach (var state in deviceState.States)
                    {
                        if (state.DriverState.IsAutomatic && state.DriverState.IsManualReset)
                            resetItem.StateNames.Add(state.DriverState.Name);
                    }
                    break;

                case AlarmType.Off:
                    break;
            }

            if (resetItem.StateNames.Count > 0)
                return resetItem;
            return null;
        }
    }
}