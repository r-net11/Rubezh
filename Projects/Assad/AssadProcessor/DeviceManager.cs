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
    public class DeviceManager
    {
        public void Config(Assad.MHconfigTypeDevice innerDevice)
        {
            Configuration.BaseDevices = new List<AssadBase>();
            Configuration.Devices = new List<AssadDevice>();
            Configuration.Zones = new List<AssadZone>();

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
            string driverId = ParceDriverId(innerDevice);
            string driverName = DriversHelper.GetDriverNameById(driverId);
            switch (driverName)
            {
                case "zone":
                    AssadZone assadZone = new AssadZone();
                    Configuration.Zones.Add(assadZone);
                    assadBase = assadZone;
                    break;
                case "monitor":
                    AssadMonitor assadMonitor = new AssadMonitor();
                    Configuration.Monitor = assadMonitor;
                    assadBase = assadMonitor;
                    break;
                default:
                    AssadDevice assadDevice = new AssadDevice();
                    assadDevice.DriverId = driverId;
                    Configuration.Devices.Add(assadDevice);
                    assadBase = assadDevice;
                    break;
            }
            assadBase.DeviceId = innerDevice.deviceId;
            Configuration.BaseDevices.Add(assadBase);
            return assadBase;
        }

        string ParceDriverId(Assad.MHconfigTypeDevice innerDevice)
        {
            string[] separators = new string[1];
            separators[0] = ".";
            string[] separatedTypes = innerDevice.type.Split(separators, StringSplitOptions.None);
            string driverId = separatedTypes[2];
            return driverId;
        }
    }
}
