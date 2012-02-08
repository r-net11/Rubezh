using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
    public class FiresecEventSubscriber : IFiresecCallback
    {
        public void DeviceStateChanged(List<DeviceState> newDeviceStates)
        {
            foreach (var newDeviceState in newDeviceStates)
            {
                var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);

                deviceState.States.Clear();
                foreach (var newState in newDeviceState.States)
                {
                    deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == newState.Code);
                    newState.DriverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == newState.Code);
                    deviceState.States.Add(newState);
                }

                deviceState.ParentStates = newDeviceState.ParentStates;
                foreach (var parentState in deviceState.ParentStates)
                {
                    parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
                    if (parentState.ParentDevice != null)
                    {
                        parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
                    }
                }

                deviceState.OnStateChanged();
                if (DeviceStateChangedEvent != null)
                    DeviceStateChangedEvent(deviceState.UID);
            }
        }

        public void DeviceParametersChanged(List<DeviceState> newDeviceStates)
        {
            foreach (var newDeviceState in newDeviceStates)
            {
                var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);
                deviceState.Parameters = newDeviceState.Parameters;

                deviceState.OnParametersChanged();
                if (DeviceParametersChangedEvent != null)
                    DeviceParametersChangedEvent(deviceState.UID);
            }
        }

        public void ZoneStateChanged(ZoneState newZoneState)
        {
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == newZoneState.No);
            zoneState.StateType = newZoneState.StateType;
            zoneState.RevertColorsForGuardZone = IsZoneOnGuard(newZoneState);

            zoneState.OnStateChanged();
            if (ZoneStateChangedEvent != null)
                ZoneStateChangedEvent(zoneState.No);
        }

        public bool IsZoneOnGuard(ZoneState zoneState)
        {
            var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneState.No);
            if (zone.ZoneType == ZoneType.Guard)
            {
                foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
                {
                    if (deviceState.Device.ZoneNo.HasValue)
                    {
                        if (deviceState.Device.ZoneNo.Value == zone.No)
                        {
                            if (deviceState.States.Any(x => x.Code == "OnGuard") == false)
                                return true;
                        }
                    }
                }
            }
            return false;
        }

        public void NewJournalRecord(JournalRecord journalRecord)
        {
            if (NewJournalRecordEvent != null)
                NewJournalRecordEvent(journalRecord);
        }

        public static event Action<JournalRecord> NewJournalRecordEvent;
        public static event Action<Guid> DeviceStateChangedEvent;
        public static event Action<Guid> DeviceParametersChangedEvent;
        public static event Action<ulong?> ZoneStateChangedEvent;
    }
}