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
        public void Progress(int stage, string comment, int percentComplete, int bytesRW)
        {
            OnOperationProgress(stage, comment, percentComplete, bytesRW);
        }

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

                OnDeviceStateChanged(deviceState.UID);
            }
        }

        public void DeviceParametersChanged(List<DeviceState> newDeviceStates)
        {
            foreach (var newDeviceState in newDeviceStates)
            {
                var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);
                deviceState.Parameters = newDeviceState.Parameters;
                OnDeviceParametersChanged(deviceState.UID);
            }
        }

        public void ZoneStateChanged(ZoneState newZoneState)
        {
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == newZoneState.No);
            zoneState.StateType = newZoneState.StateType;
            zoneState.RevertColorsForGuardZone = IsZoneOnGuard(newZoneState);
            OnZoneStateChanged(zoneState.No);
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
                        if (deviceState.Device.ZoneNo.Value == zone.No.Value)
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
            OnNewJournalRecordEvent(journalRecord);
        }

        public delegate bool ProgressDelegate(int stage, string comment, int percentComplete, int bytesRW);

        public static event ProgressDelegate OperationProgress;
        public static bool OnOperationProgress(int stage, string comment, int percentComplete, int bytesRW)
        {
            if (OperationProgress != null)
                return OperationProgress(stage, comment, percentComplete, bytesRW);
            return false;
        }

        public static event Action<JournalRecord> NewJournalRecordEvent;
        public static void OnNewJournalRecordEvent(JournalRecord journalRecord)
        {
            if (NewJournalRecordEvent != null)
                NewJournalRecordEvent(journalRecord);
        }

        public static event Action<Guid> DeviceStateChangedEvent;
        public static void OnDeviceStateChanged(Guid deviceUID)
        {
            if (DeviceStateChangedEvent != null)
                DeviceStateChangedEvent(deviceUID);

            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
            deviceState.OnStateChanged();
        }

        public static event Action<Guid> DeviceParametersChangedEvent;
        public static void OnDeviceParametersChanged(Guid deviceUID)
        {
            if (DeviceParametersChangedEvent != null)
                DeviceParametersChangedEvent(deviceUID);
        }

        public static event Action<ulong?> ZoneStateChangedEvent;
        public static void OnZoneStateChanged(ulong? zoneNo)
        {
            if (ZoneStateChangedEvent != null)
                ZoneStateChangedEvent(zoneNo);
        }
    }
}