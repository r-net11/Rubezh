using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common;

namespace Client
{
    public static class DeviceManager
    {
        public static List<Device> Devices { get; set; }

        public static void Config(x.MHconfigTypeDevice innerDevice, bool all)
        {
            if (all == false)
            {
                throw (new NotImplementedException("Hot configuration in not enabled not"));
            }

            Devices = new List<Device>();

            Device device = new Device();
            device.InnerDevice = innerDevice;
            device.Parent = null;
            Devices.Add(device);
            AddChild(innerDevice, device);
        }

        static void AddChild(x.MHconfigTypeDevice innerDevice, Device parent)
        {
            if (innerDevice.device != null)
                foreach (x.MHconfigTypeDevice innerChild in innerDevice.device)
                {
                    Device child = new Device();
                    child.InnerDevice = innerChild;
                    child.Parent = parent;
                    Devices.Add(child);
                    AddChild(innerChild, child);
                }
        }

        public static void EventAggregator_PropertyChanged(string Address, string ParentAddress, ComState newState, int DeviceState)
        {
            if (ParentAddress == "0")
            {
                ParentAddress = null;
            }

            Device device = Devices.First(x => ((x.Address == Address) && (x.ParentAddress == ParentAddress)));
            device.State.CurrentClass = DeviceState;
            device.FireEvent(newState.Name);
        }

        public static void RemoveDevice(x.MHremoveDeviceType content, string refMessageId)
        {
            throw (new NotImplementedException("Hot configuration in not enabled not"));
        }

        public static x.DeviceType[] QueryState(x.MHqueryStateType content, string refMessageId)
        {
            Device device = Devices.First(a => a.DeviceId == content.deviceId);
            List<TreeBase> devices = device.FindAllChildren();

            x.DeviceType[] deviceItems = new x.DeviceType[devices.Count];
            for (int i = 0; i < devices.Count; i++)
            {
                deviceItems[i] = (devices[i] as Device).GetInnerStates();
            }

            return deviceItems;
        }

        public static void ExecuteCommand(x.MHdeviceControlType controlType, string refMessageId)
        {
            Device device = Devices.First(x => x.DeviceId == controlType.deviceId);
            device.ExecuteCommand(controlType, refMessageId);
        }
    }
}
