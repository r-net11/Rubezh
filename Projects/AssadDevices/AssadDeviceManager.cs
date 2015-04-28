using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace AssadDdevices
{
    public static class AssadDeviceManager
    {
        public static List<AssadDevice> Devices { get; set; }

        public static void Config(x.MHconfigTypeDevice innerDevice, bool all)
        {
            if (all == false)
            {
                throw (new NotImplementedException("Hot configuration in not enabled not"));
            }

            Devices = new List<AssadDevice>();

            AssadDevice device = new AssadDevice();
            device.InnerDevice = innerDevice;
            device.Parent = null;
            Devices.Add(device);
            AddChild(innerDevice, device);
        }

        static void AddChild(x.MHconfigTypeDevice innerDevice, AssadDevice parent)
        {
            if (innerDevice.device != null)
                foreach (x.MHconfigTypeDevice innerChild in innerDevice.device)
                {
                    AssadDevice child = new AssadDevice();
                    child.InnerDevice = innerChild;
                    child.Parent = parent;
                    Devices.Add(child);
                    AddChild(innerChild, child);
                }
        }

        public static void RemoveDevice(x.MHremoveDeviceType content, string refMessageId)
        {
            throw (new NotImplementedException("Hot configuration in not enabled not"));
        }

        public static x.DeviceType[] QueryState(x.MHqueryStateType content, string refMessageId)
        {
            AssadDevice device = Devices.First(a => a.DeviceId == content.deviceId);
            List<TreeBase> devices = device.FindAllChildren();

            x.DeviceType[] deviceItems = new x.DeviceType[devices.Count];
            for (int i = 0; i < devices.Count; i++)
            {
                deviceItems[i] = (devices[i] as AssadDevice).GetInnerStates();
            }

            return deviceItems;
        }

        public static void ExecuteCommand(x.MHdeviceControlType controlType, string refMessageId)
        {
            AssadDevice device = Devices.First(x => x.DeviceId == controlType.deviceId);
            device.ExecuteCommand(controlType, refMessageId);
        }
    }
}
