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
            ServiceClient.CurrentStates.DeviceStateChanged += new Action<string>(ServiceClient_DeviceChanged);
            ServiceClient.CurrentStates.ZoneStateChanged += new Action<string>(CurrentStates_ZoneStateChanged);
            ServiceClient.CurrentStates.NewJournalEvent += new Action<Firesec.ReadEvents.journalType>(CurrentStates_NewJournalEvent);
        }

        void ServiceClient_DeviceChanged(string path)
        {
            DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == path);
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
            ZoneState zoneState = ServiceClient.CurrentStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
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
            if (ServiceClient.CurrentConfiguration.AllDevices.Any(x => x.DatabaseId == dataBaseId))
            {
                Device device = ServiceClient.CurrentConfiguration.AllDevices.FirstOrDefault(x => x.DatabaseId == dataBaseId);
                DeviceState deviceState = ServiceClient.CurrentStates.DeviceStates.FirstOrDefault(x => x.Path == device.Path);

                if ((AssadConfiguration.Devices != null) && (AssadConfiguration.Devices.Any(x => x.Path == device.Path)))
                {
                    AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == device.Path);

                    string eventName = StateHelper.GetState(Convert.ToInt32(journalItem.IDTypeEvents));
                    Assad.CPeventType eventType = assadBase.CreateEvent(eventName);
                    NetManager.Send(eventType, null);
                }
            }
        }

        //void SetMonitorState()
        //{
        //    int Alarm = 0;
        //    int Warning = 0;
        //    int Failure = 0;
        //    int Service = 0;
        //    int Off = 0;
        //    int Unknown = 0;
        //    int Info = 0;
        //    int Norm = 0;
        //    foreach (DeviceState deviceState in ServiceClient.CurrentStates.DeviceStates)
        //    {
        //        switch (deviceState.State)
        //        {
        //            case "Тревога":
        //                Alarm++;
        //                break;

        //            case "Внимание (предтревожное)":
        //                Warning++;
        //                break;

        //            case "Неисправность":
        //                Failure++;
        //                break;

        //            case "Требуется обслуживание":
        //                Service++;
        //                break;

        //            case "Обход устройств":
        //                Off++;
        //                break;

        //            case "Неопределено":
        //                Unknown++;
        //                break;

        //            case "Норма(*)":
        //                Info++;
        //                break;

        //            case "Норма":
        //                Norm++;
        //                break;

        //            default:
        //                break;
        //        }
        //    }



        //    if (Alarm > 0)
        //    {
        //    }
        //}
    }
}
