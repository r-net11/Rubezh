using System;
using System.Linq;
using System.ServiceModel;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
    [CallbackBehavior(ConcurrencyMode=ConcurrencyMode.Single)]
    public class FiresecEventSubscriber : IFiresecCallback
    {
        public void DeviceStateChanged(DeviceState newDeviceState)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);
            deviceState.CopyFrom(newDeviceState);

            foreach (var state in deviceState.States)
            {
                var driverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == state.Code);
                state.DriverState = driverState;
            }

            foreach (var parentState in deviceState.ParentStates)
            {
                parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
                var driverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
                parentState.DriverState = driverState;
            }

            OnDeviceStateChanged(deviceState.UID);
        }

        public void DeviceParametersChanged(DeviceState newDeviceState)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);
            deviceState.CopyFrom(newDeviceState);
            OnDeviceParametersChanged(deviceState.UID);
        }

        public void ZoneStateChanged(ZoneState newZoneState)
        {
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == newZoneState.No);
            zoneState.StateType = newZoneState.StateType;
            OnZoneStateChanged(zoneState.No);
        }

        public void NewJournalRecord(JournalRecord journalRecord)
        {
            OnNewJournalRecordEvent(journalRecord);
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

        public static event Action<string> ZoneStateChangedEvent;
        public static void OnZoneStateChanged(string zoneNo)
        {
            if (ZoneStateChangedEvent != null)
                ZoneStateChangedEvent(zoneNo);
        }
    }
}
