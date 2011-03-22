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
            ServiceClient.Configuration.DeviceStateChanged += new Action<Device>(ServiceClient_DeviceChanged);
        }

        void ServiceClient_DeviceChanged(Device device)
        {
            bool assadReady = (AssadConfiguration.Devices == null) ? false : true;

            if (assadReady)
            {
                AssadBase assadBase = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == device.Path);
                if (assadBase != null)
                {
                    assadBase.MainState = device.State;

                    string eventName = null;
                    //if (device.StateChanged)
                    //    eventName = device.State;

                    assadBase.States = new List<string>();
                    foreach(string state in device.States)
                    {
                        assadBase.States.Add(state);
                    }

                    if ((device.StateChanged) || (device.StatesChanged) || (device.VisibleParameterChanged))
                    {
                        Assad.CPeventType eventType = assadBase.CreateEvent(eventName);
                        NetManager.Send(eventType, null);
                    }
                }
            }
        }
    }
}
