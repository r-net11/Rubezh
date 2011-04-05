using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using ServiceApi;
using ClientApi;

namespace AssadProcessor
{
    public class AssadWatcher
    {
        internal void Start()
        {
            ServiceClient.CurrentStates.DeviceStateChanged += new Action<DeviceState>(ServiceClient_DeviceChanged);
            ServiceClient.CurrentStates.ZoneStateChanged += new Action<ZoneState>(CurrentStates_ZoneStateChanged);
        }

        void ServiceClient_DeviceChanged(DeviceState deviceState)
        {
            bool assadReady = (AssadConfiguration.Devices == null) ? false : true;

            if (assadReady)
            {
                AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == deviceState.Path);
                if (assadBase != null)
                {
                    assadBase.MainState = deviceState.State;

                    string eventName = null;
                    if (deviceState.ChangeEntities.IsNewEvent)
                        eventName = deviceState.State;

                    assadBase.States = new List<string>();
                    if (deviceState.States != null)
                        foreach (string state in deviceState.States)
                        {
                            assadBase.States.Add(state);
                        }

                    if ((deviceState.ChangeEntities.IsNewEvent) || (deviceState.ChangeEntities.StateChanged) || (deviceState.ChangeEntities.StatesChanged) || (deviceState.ChangeEntities.VisibleParameterChanged))
                    {
                        Assad.CPeventType eventType = assadBase.CreateEvent(eventName);
                        NetManager.Send(eventType, null);
                    }
                }
            }
        }

        void CurrentStates_ZoneStateChanged(ZoneState zoneState)
        {
            bool assadReady = (AssadConfiguration.Devices == null) ? false : true;

            if (assadReady)
            {
                AssadZone assadZone = null;
                foreach (AssadBase assadBase in AssadConfiguration.Devices)
                {
                    if (assadBase is AssadZone)
                    {
                        if ((assadBase as AssadZone).ZoneNo == zoneState.No)
                            assadZone = assadBase as AssadZone;
                    }
                }

                //AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == zoneState.No);
                if (assadZone != null)
                {
                    assadZone.MainState = zoneState.State;

                    //string eventName = null;
                    //if (deviceState.ChangeEntities.IsNewEvent)
                    string eventName = zoneState.State;

                    //assadBase.States = new List<string>();
                    //if (zoneState.States != null)
                    //foreach(string state in zoneState.States)
                    //{
                    //    assadBase.States.Add(state);
                    //}

                    //if ((deviceState.ChangeEntities.IsNewEvent) || (deviceState.ChangeEntities.StateChanged) || (deviceState.ChangeEntities.StatesChanged) || (deviceState.ChangeEntities.VisibleParameterChanged))
                    //{
                    Assad.CPeventType eventType = assadZone.CreateEvent(eventName);
                    NetManager.Send(eventType, null);
                    //}
                }
            }
        }
    }
}
