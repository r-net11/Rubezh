using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace AssadDevices
{
    public class AssadDeviceManager
    {
        // передается древовидная структура устройств, пришедших из ассада, на основании которой нужно построить
        // свое локальной дерево устройств
        // устройства ассада могут представлять из себя либо устройство, либо зону
        // это можно определить по свойству innerDevice.type

        public void Config(Assad.MHconfigTypeDevice innerDevice)
        {
            AssadConfiguration.IsValid = true;
            AssadConfiguration.Devices = new List<AssadBase>();
            AssadConfiguration.InvalidDevices = new List<AssadBase>();

            AssadBase device = AssadDeviceFactory.Create(innerDevice);
            device.Parent = null;
            device.SetInnerDevice(innerDevice);
            device.SetPath();
            AssadConfiguration.Devices.Add(device);
            AddChild(innerDevice, device);
        }

        void AddChild(Assad.MHconfigTypeDevice innerDevice, AssadBase parent)
        {
            if (innerDevice.device != null)
                foreach (Assad.MHconfigTypeDevice innerChild in innerDevice.device)
                {
                    AssadBase child = AssadDeviceFactory.Create(innerChild);
                    child.Parent = parent;
                    parent.Children.Add(child);
                    child.SetInnerDevice(innerChild);
                    child.SetPath();
                    AssadConfiguration.Devices.Add(child);
                    AddChild(innerChild, child);
                }
        }

        public Assad.DeviceType[] QueryState(Assad.MHqueryStateType content)
        {
            AssadBase device;
            try
            {
                device = AssadConfiguration.Devices.First(a => a.DeviceId == content.deviceId);
            }
            catch
            {
                Trace.WriteLine("DEBUG: QueryState Fail");
                return null;
            }
            List<AssadBase> devices = device.FindAllChildren();

            Assad.DeviceType[] deviceItems = new Assad.DeviceType[devices.Count];
            for (int i = 0; i < devices.Count; i++)
            {
                deviceItems[i] = devices[i].GetInnerStates();
            }

            return deviceItems;
        }

        public Assad.DeviceType QueryAbility(Assad.MHqueryAbilityType content)
        {
            AssadBase device = AssadConfiguration.Devices.First(a => a.DeviceId == content.deviceId);
            Assad.DeviceType ability = device.QueryAbility();
            return ability;
        }
    }
}
