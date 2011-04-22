using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using FiresecClient;
using Firesec;

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
            DeviceState deviceState = FiresecManager.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == path);
            AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == deviceState.Path);
            if (assadBase != null)
            {
                Assad.CPeventType eventType = assadBase.CreateEvent(null);
                NetManager.Send(eventType, null);
            }

            if (AssadConfiguration.Devices.Any(x => x is AssadMonitor))
            {
                AssadMonitor assadMonitor = (AssadMonitor)AssadConfiguration.Devices.FirstOrDefault(x => x is AssadMonitor);

                Assad.CPeventType monitorEventType = assadMonitor.CreateEvent(null);
                NetManager.Send(monitorEventType, null);
            }
        }

        void CurrentStates_ZoneStateChanged(string zoneNo)
        {
            ZoneState zoneState = FiresecManager.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
            AssadZone assadZone = null;
            if (AssadConfiguration.Devices != null)
            {
                foreach (AssadBase assadBase in AssadConfiguration.Devices)
                {
                    if (assadBase is AssadZone)
                    {
                        if ((assadBase as AssadZone).ZoneNo == zoneState.No)
                            assadZone = assadBase as AssadZone;
                    }
                }
            }

            if (assadZone != null)
            {
                Assad.CPeventType eventType = assadZone.CreateEvent(null);
                NetManager.Send(eventType, null);
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
                    AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == device.Path);

                    string eventName = StateHelper.GetState(Convert.ToInt32(journalItem.IDTypeEvents));
                    Assad.CPeventType eventType = assadBase.CreateEvent(eventName);
                    NetManager.Send(eventType, null);
                }
            }
        }
    }
}
