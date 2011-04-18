using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using ClientApi;
using FiresecMetadata;

namespace AssadProcessor
{
    public class AssadWatcher
    {
        internal void Start()
        {
            ServiceClient.CurrentStates.DeviceStateChanged += new Action<DeviceState>(ServiceClient_DeviceChanged);
            ServiceClient.CurrentStates.ZoneStateChanged += new Action<ZoneState>(CurrentStates_ZoneStateChanged);
            ServiceClient.CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void ServiceClient_DeviceChanged(DeviceState deviceState)
        {
            if ((AssadConfiguration.Devices != null) && (AssadConfiguration.Devices.Any(x => x.Path == deviceState.Path)))
            {
                AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == deviceState.Path);
                if (assadBase != null)
                {

                    // МЕТОД - КОПИРОВАТЬ ДАННЫЕ ИЗ КОНФИГУРАЦИИ

                    assadBase.MainState = deviceState.State;

                    assadBase.States = new List<string>();
                    if (deviceState.States != null)
                        foreach (string state in deviceState.States)
                        {
                            assadBase.States.Add(state);
                        }

                    if ((deviceState.ChangeEntities.StateChanged) || (deviceState.ChangeEntities.StatesChanged) || (deviceState.ChangeEntities.VisibleParameterChanged))
                    {
                        Assad.CPeventType eventType = assadBase.CreateEvent(null);
                        NetManager.Send(eventType, null);
                    }
                }
            }
        }

        void CurrentStates_ZoneStateChanged(ZoneState zoneState)
        {
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
                assadZone.MainState = zoneState.State;
                string eventName = null;// zoneState.State;
                Assad.CPeventType eventType = assadZone.CreateEvent(eventName);
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
            if (ServiceClient.CurrentConfiguration.AllDevices.Any(x => x.DatabaseId == dataBaseId))
            {
                Device device = ServiceClient.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == device.Path);

                if ((AssadConfiguration.Devices != null) && (AssadConfiguration.Devices.Any(x => x.Path == device.Path)))
                {
                    AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == device.Path);

                    string eventName = StateHelper.GetState(Convert.ToInt32(journalItem.IDTypeEvents));// deviceState.State;
                    Assad.CPeventType eventType = assadBase.CreateEvent(eventName);
                    NetManager.Send(eventType, null);
                }
            }
        }
    }
}
