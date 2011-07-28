using System;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
    public class FiresecEventSubscriber : IFiresecCallback
    {
        public void DeviceStateChanged(string deviceId)
        {
            OnDeviceStateChanged(deviceId);
        }

        public void DeviceParametersChanged(string deviceId)
        {
            OnDeviceParametersChanged(deviceId);
        }

        public void ZoneStateChanged(string zoneNo)
        {
            OnZoneStateChanged(zoneNo);
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

            var deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == deviceId);
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
