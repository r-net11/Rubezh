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
            ServiceClient.DeviceStateChanged += new Action<Device>(ServiceClient_DeviceChanged);
        }

        void ServiceClient_DeviceChanged(Device device)
        {
            bool assadReady = (AssadConfiguration.Devices == null) ? false : true;

                if (assadReady)
                {
                    AssadBase assadDevice = Helper.ConvertDevice(device);
                    assadDevice.State.State = device.State;
                    foreach (string lastEvent in device.LastEvents)
                    {
                        Assad.CPeventType eventType = assadDevice.CreateEvent(lastEvent);
                        NetManager.Send(eventType, null);
                    }
                }
        }
    }
}
