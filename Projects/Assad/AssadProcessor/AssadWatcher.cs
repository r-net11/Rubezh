using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using Firesec;
using AssadProcessor.Devices;

namespace AssadProcessor
{
    public class AssadWatcher
    {
        internal void Start()
        {
            FiresecManager.CurrentStates.DeviceStateChanged += new Action<string>(ServiceClient_DeviceChanged);
            FiresecManager.CurrentStates.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
            CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void ServiceClient_DeviceChanged(string path)
        {
            AssadDevice assadDevice = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == path);
            if (assadDevice != null)
            {
                Assad.CPeventType eventType = assadDevice.CreateEvent(null);
                NetManager.Send(eventType, null);
            }

            if (AssadConfiguration.Monitor != null)
            {
                Assad.CPeventType monitorEventType = AssadConfiguration.Monitor.CreateEvent(null);
                NetManager.Send(monitorEventType, null);
            }
        }

        void CurrentStates_ZoneStateChanged(string zoneNo)
        {
            if (AssadConfiguration.Zones != null)
            {
                AssadZone assadZone = AssadConfiguration.Zones.FirstOrDefault(x => x.ZoneNo == zoneNo);
                if (assadZone != null)
                {
                    Assad.CPeventType eventType = assadZone.CreateEvent(null);
                    NetManager.Send(eventType, null);
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
            if (FiresecManager.CurrentConfiguration.AllDevices.Any(x => x.DatabaseId == dataBaseId))
            {
                Device device = FiresecManager.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
                DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == device.Path);

                if ((AssadConfiguration.Devices != null) && (AssadConfiguration.Devices.Any(x => x.Path == device.Path)))
                {
                    AssadDevice assadDevice = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == device.Path);

                    string eventName = StateHelper.GetState(Convert.ToInt32(journalItem.IDTypeEvents));
                    Assad.CPeventType eventType = assadDevice.CreateEvent(eventName);
                    NetManager.Send(eventType, null);
                }
            }
        }
    }
}
