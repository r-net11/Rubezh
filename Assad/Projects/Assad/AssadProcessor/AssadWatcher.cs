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
                    //if (device.StateChanged)
                    //    eventName = device.State;

                    assadBase.States = new List<string>();
                    foreach(string state in deviceState.States)
                    {
                        assadBase.States.Add(state);
                    }

                    if ((deviceState.ChangeEntities.StateChanged) || (deviceState.ChangeEntities.StatesChanged) || (deviceState.ChangeEntities.VisibleParameterChanged))
                    {
                        Assad.CPeventType eventType = assadBase.CreateEvent(eventName);
                        NetManager.Send(eventType, null);
                    }
                }
            }
        }
    }
}
