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
            FiresecManager.States.DeviceStateChanged += new Action<string>(ServiceClient_DeviceChanged);
            FiresecManager.States.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
            //FiresecManager.States.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void ServiceClient_DeviceChanged(string id)
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

        void CurrentStates_ZoneStateChanged(string zoneNo)
        {
            var assadZone = Configuration.Zones.FirstOrDefault(x => x.ZoneNo == zoneNo);
            if (assadZone != null)
            {
                assadZone.FireEvent(null);
            }
        }

        void CurrentStates_NewJournalEvent(JournalItem journalItem)
        {
            if (journalItem.DeviceDatabaseId != null)
            {
                SendEvent(journalItem, journalItem.DeviceDatabaseId);
            }
            if (journalItem.PanelDatabaseId != null)
            {
                SendEvent(journalItem, journalItem.PanelDatabaseId);
            }
        }

        void SendEvent(JournalItem journalItem, string dataBaseId)
        {
            var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
            if (device != null)
            {
                var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == device.Id);
                if (assadDevice != null)
                {
                    string eventName = journalItem.State.ToString();
                    assadDevice.FireEvent(eventName);
                }
            }
        }
    }
}
