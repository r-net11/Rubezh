using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AssadDevices;
using ServiceApi;
using ClientApi;

namespace AssadProcessor
{
    public class AssadWather
    {
        internal void Start()
        {
            ServiceClient.DeviceStateChanged += new Action<ShortDeviceState>(ServiceClient_DeviceChanged);
        }

        void ServiceClient_DeviceChanged(ShortDeviceState shortDeviceState)
        {
            bool assadReady = (AssadConfiguration.Devices == null) ? false : true;

            if (assadReady)
            {
                AssadBase assadDevice = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == shortDeviceState.Path);
                if (assadDevice != null)
                {
                    assadDevice.State.State = shortDeviceState.State;

                    string eventName = null;
                    if (shortDeviceState.StateChanged)
                        eventName = shortDeviceState.State;

                    if ((shortDeviceState.StateChanged) || (shortDeviceState.StatesChanged) || (shortDeviceState.VisibleParameterChanged))
                    {
                        Assad.CPeventType eventType = assadDevice.CreateEvent(eventName);
                        NetManager.Send(eventType, null);
                    }
                }
            }
        }
    }
}
