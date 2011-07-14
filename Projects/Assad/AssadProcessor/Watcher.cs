using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using Firesec;
using AssadProcessor.Devices;
using FiresecClient.Models;

namespace AssadProcessor
{
    public class Watcher
    {
        internal void Start()
        {
            FiresecManager.States.DeviceStateChanged += new Action<string>(ServiceClient_DeviceChanged);
            FiresecManager.States.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
            FiresecManager.States.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
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

        void CurrentStates_NewJournalEvent(Firesec.ReadEvents.journalType journalItem)
        {
            if (journalItem.IDDevices != null)
            {
                SendEvent(journalItem, journalItem.IDDevices);
            }
            if (journalItem.IDDevicesSource != null)
            {
                SendEvent(journalItem, journalItem.IDDevicesSource);
            }
        }

        void SendEvent(Firesec.ReadEvents.journalType journalItem, string dataBaseId)
        {
            var device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
            if (device != null)
            {
                var assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == device.Id);
                if (assadDevice != null)
                {
                    string eventName = new State(Convert.ToInt32(journalItem.IDTypeEvents)).ToString();
                    assadDevice.FireEvent(eventName);
                }
            }
        }
    }
}
