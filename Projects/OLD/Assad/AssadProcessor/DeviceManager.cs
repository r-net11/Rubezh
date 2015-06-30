﻿using AssadProcessor.Devices;
using FiresecAPI.Models;

namespace AssadProcessor
{
	public class DeviceManager
	{
		public void Config(Assad.MHconfigTypeDevice innerDevice)
		{
			AssadBase device = Create(innerDevice);
			device.Parent = null;
			device.SetInnerDevice(innerDevice);
			AddChild(innerDevice, device);
		}

		void AddChild(Assad.MHconfigTypeDevice innerDevice, AssadBase parent)
		{
			if (innerDevice.device != null)
				foreach (var innerChild in innerDevice.device)
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
					var assadZone = new AssadZone();
					Configuration.Zones.Add(assadZone);
					assadBase = assadZone;
					break;
				case "monitor":
					var assadMonitor = new AssadMonitor();
					Configuration.Monitor = assadMonitor;
					assadBase = assadMonitor;
					break;
				default:
					var assadDevice = new AssadDevice();
					assadDevice.DriverId = driverId;
					Configuration.Devices.Add(assadDevice);
					assadBase = assadDevice;
					break;
			}
			assadBase.DeviceId = innerDevice.deviceId;
			Configuration.BaseDevices.Add(assadBase);
			return assadBase;
		}

		string GetDriverId(Assad.MHconfigTypeDevice innerDevice)
		{
			string[] separatedTypes = innerDevice.type.Split('.');
			string driverId = separatedTypes[2];
			return driverId;
		}
	}
}