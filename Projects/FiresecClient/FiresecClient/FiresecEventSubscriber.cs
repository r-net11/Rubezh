using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
    public class FiresecEventSubscriber : IFiresecCallback
    {
        public void DeviceStateChanged(DeviceState newDeviceState)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == newDeviceState.Id);
            deviceState.CopyFrom(newDeviceState);
            OnDeviceStateChanged(deviceState.Id);
        }

        public void DeviceParametersChanged(DeviceState newDeviceState)
        {
            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == newDeviceState.Id);
            deviceState.CopyFrom(newDeviceState);
            OnDeviceParametersChanged(deviceState.Id);
        }

        public void ZoneStateChanged(ZoneState newZoneState)
        {
            var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == newZoneState.No);
            zoneState.State = newZoneState.State;
            OnZoneStateChanged(zoneState.No);
        }

        public void NewJournalItem(JournalItem journalItem)
        {
            OnNewJournalItemEvent(journalItem);
        }

        public static event Action<JournalItem> NewJournalItemEvent;
        public static void OnNewJournalItemEvent(JournalItem journalItem)
        {
            if (NewJournalItemEvent != null)
                NewJournalItemEvent(journalItem);
        }

        public static event Action<string> DeviceStateChangedEvent;
        public static void OnDeviceStateChanged(string deviceId)
        {
            if (DeviceStateChangedEvent != null)
                DeviceStateChangedEvent(deviceId);

            var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.Id == deviceId);
            deviceState.OnStateChanged();
        }

        public static event Action<string> DeviceParametersChangedEvent;
        public static void OnDeviceParametersChanged(string deviceId)
        {
            if (DeviceParametersChangedEvent != null)
                DeviceParametersChangedEvent(deviceId);
        }

        public static event Action<string> ZoneStateChangedEvent;
        public static void OnZoneStateChanged(string zoneNo)
        {
            if (ZoneStateChangedEvent != null)
                ZoneStateChangedEvent(zoneNo);
        }
    }
}
