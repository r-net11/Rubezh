using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;
using AssadProcessor.Devices;
using Firesec;

namespace AssadProcessor
{
    public class AssadDeviceManager
    {
        // передается древовидная структура устройств, пришедших из ассада, на основании которой нужно построить
        // свое локальной дерево устройств
        // устройства ассада могут представлять из себя либо устройство, либо зону
        // это можно определить по свойству innerDevice.type

        public void Config(Assad.MHconfigTypeDevice innerDevice)
        {
            AssadConfiguration.BaseDevices = new List<AssadBase>();
            AssadConfiguration.Devices = new List<AssadDevice>();
            AssadConfiguration.Zones = new List<AssadZone>();

            AssadBase device = Create(innerDevice);
            device.Parent = null;
            device.SetInnerDevice(innerDevice);
            AddChild(innerDevice, device);
        }

        void AddChild(Assad.MHconfigTypeDevice innerDevice, AssadBase parent)
        {
            if (innerDevice.device != null)
                foreach (Assad.MHconfigTypeDevice innerChild in innerDevice.device)
                {
                    AssadBase child = Create(innerChild);
                    child.Parent = parent;
                    parent.Children.Add(child);
                    child.SetInnerDevice(innerChild);
                    AddChild(innerChild, child);
                }
        }

        AssadBase Create(Assad.MHconfigTypeDevice innerDevice)
        {
            AssadBase assadBase;
            string driverId = GetDriverId(innerDevice);
            string driverName = DriversHelper.GetDriverNameById(driverId);
            switch (driverName)
            {
                case "zone":
                    assadBase = new AssadZone();
                    AssadConfiguration.Zones.Add(assadBase as AssadZone);
                    break;
                case "monitor":
                    assadBase = new AssadMonitor();
                    AssadConfiguration.Monitor = assadBase as AssadMonitor;
                    break;
                default:
                    assadBase = new AssadDevice();
                    (assadBase as AssadDevice).DriverId = driverId;
                    AssadConfiguration.Devices.Add(assadBase as AssadDevice);
                    break;
            }
            AssadConfiguration.BaseDevices.Add(assadBase);
            return assadBase;
        }

        string GetDriverId(Assad.MHconfigTypeDevice innerDevice)
        {
            string[] separators = new string[1];
            separators[0] = ".";
            string[] separatedTypes = innerDevice.type.Split(separators, StringSplitOptions.None);
            string driverId = separatedTypes[2];
            return driverId;
        }

        // MOVE TO CONTROLLER

        public Assad.DeviceType[] QueryState(Assad.MHqueryStateType content)
        {
            AssadBase device;
            try
            {
                device = AssadConfiguration.BaseDevices.First(a => a.DeviceId == content.deviceId);
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
            AssadBase device = AssadConfiguration.BaseDevices.First(a => a.DeviceId == content.deviceId);
            Assad.DeviceType ability = device.QueryAbility();
            return ability;
        }
    }
}
