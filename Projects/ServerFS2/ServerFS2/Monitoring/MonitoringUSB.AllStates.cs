using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using ServerFS2.Service;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringUSB
	{
		public void SetAllInitializing()
		{
			var deviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				deviceStates.Add(device.DeviceState);
				device.DeviceState.IsInitializing = true;
				var deviceStatesManager = new DeviceStatesManager();
				deviceStatesManager.ChangeDeviceStates(device, true);
			}
			CallbackManager.DeviceStateChanged(deviceStates);
			ZoneStateManager.ChangeOnDeviceState();
		}

		public void RemoveAllInitializing()
		{
			var deviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				deviceStates.Add(device.DeviceState);
				device.DeviceState.IsInitializing = false;
				var deviceStatesManager = new DeviceStatesManager();
				deviceStatesManager.ChangeDeviceStates(device, true);
			}
			CallbackManager.DeviceStateChanged(deviceStates);
			ZoneStateManager.ChangeOnDeviceState();
		}

		void SetStateByName(string stateName, Device device)
		{
			var state = device.Driver.States.FirstOrDefault(y => y.Name == stateName);
			var deviceDriverState = new DeviceDriverState { DriverState = state, Time = DateTime.Now };
			device.DeviceState.States = new List<DeviceDriverState> { deviceDriverState };
		}
	}
}