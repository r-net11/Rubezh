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
            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void ServiceClient_DeviceChanged(string id)
        {
            AssadDevice assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == id);
            if (assadDevice != null)
            {
                assadDevice.FireEvent(null);
            }

            if (Configuration.Monitor != null)
            {
                // отладочное событие
                // Configuration.Monitor.FireEvent("Изменено состояние монитора");
                Configuration.Monitor.FireEvent(null);
            }
        }

        void CurrentStates_ZoneStateChanged(string zoneNo)
        {
            if (Configuration.Zones != null)
            {
                AssadZone assadZone = Configuration.Zones.FirstOrDefault(x => x.ZoneNo == zoneNo);
                if (assadZone != null)
                {
                    assadZone.FireEvent(null);
                }
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
            if (FiresecManager.Configuration.Devices.Any(x => x.DatabaseId == dataBaseId))
            {
                Device device = FiresecManager.Configuration.Devices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
                DeviceState deviceState = FiresecManager.States.DeviceStates.FirstOrDefault(x => x.Id == device.Id);

                if ((Configuration.Devices != null) && (Configuration.Devices.Any(x => x.Id == device.Id)))
                {
                    AssadDevice assadDevice = Configuration.Devices.FirstOrDefault(x => x.Id == device.Id);

                    string eventName = new State(Convert.ToInt32(journalItem.IDTypeEvents)).ToString();
                    assadDevice.FireEvent(eventName);
                }
            }
        }
    }
}
