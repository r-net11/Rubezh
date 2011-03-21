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
            ServiceClient.Configuration.DeviceStateChanged += new Action<Device>(ServiceClient_DeviceChanged);
        }

        void ServiceClient_DeviceChanged(Device device)
        {
            bool assadReady = (AssadConfiguration.Devices == null) ? false : true;

            if (assadReady)
            {
                AssadBase assadDevice = AssadConfiguration.Devices.FirstOrDefault(x => x.Path == device.Path);
                if (assadDevice != null)
                {
                    assadDevice.State.State = device.State;

                    string eventName = null;
                    if (device.StateChanged)
                        eventName = device.State;

                    if ((device.StateChanged) || (device.StatesChanged) || (device.VisibleParameterChanged))
                    {
                        Assad.CPeventType eventType = assadDevice.CreateEvent(eventName);
                        NetManager.Send(eventType, null);
                    }
                }
            }
        }
    }
}
