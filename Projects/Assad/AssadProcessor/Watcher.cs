using System;
using System.Linq;
using FiresecClient;
using FiresecAPI.Models;

namespace AssadProcessor
{
    public class Watcher
    {
        internal void Start()
        {
            FiresecEventSubscriber.DeviceStateChangedEvent += new Action<string>(OnDeviceStateChangedEvent);
            FiresecEventSubscriber.ZoneStateChangedEvent += new Action<string>(OnZoneStateChangedEvent);
            FiresecEventSubscriber.NewJournalItemEvent += new Action<JournalRecord>(OnNewJournalItemEvent);
        }

        void OnDeviceStateChangedEvent(string id)
        {
            var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == id);
            if (assadDevice != null)
            {
                assadDevice.FireEvent(null);
            }

            if (Configuration.Monitor != null)
            {
                Configuration.Monitor.FireEvent(null);
            }
        }

        void OnZoneStateChangedEvent(string zoneNo)
        {
            var assadZone = Configuration.Zones.FirstOrDefault(x => x.ZoneNo == zoneNo);
            if (assadZone != null)
            {
                assadZone.FireEvent(null);
            }
        }

        void OnNewJournalItemEvent(JournalRecord journalRecord)
        {
            if (journalRecord.DeviceDatabaseId != null)
            {
                SendEvent(journalRecord, journalRecord.DeviceDatabaseId);
            }
            if (journalRecord.PanelDatabaseId != null)
            {
                SendEvent(journalRecord, journalRecord.PanelDatabaseId);
            }
        }

        void SendEvent(JournalRecord journalRecord, string dataBaseId)
        {
            var device = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
            if (device != null)
            {
                var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == device.Id);
                if (assadDevice != null)
                {
                    string eventName = journalRecord.State.ToString();
                    assadDevice.FireEvent(eventName);
                }
            }
        }
    }
}
