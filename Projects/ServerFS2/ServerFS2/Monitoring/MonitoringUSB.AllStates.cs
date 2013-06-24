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
		public void SetAllMonitoringDisabled()
		{
			var changedDeviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.IsParentMonitoringDisabled)
				{
					SetStateByName("Мониторинг устройства отключен", device, ref changedDeviceStates);
					ConfigurationManager.DeviceConfiguration.Devices.Where(y => y.ParentPanel == device).ToList().ForEach(y => SetStateByName("Мониторинг устройства отключен", y, ref changedDeviceStates));
					device.OnChanged();
				}
			}
			CallbackManager.DeviceStateChanged(changedDeviceStates);
			ZoneStateManager.ChangeOnDeviceState();
		}

		public void SetAllInitializing()
		{
			var changedDeviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				SetStateByName("Устройство инициализируется", device, ref changedDeviceStates);
				device.DeviceState.OnStateChanged();
			}
			CallbackManager.DeviceStateChanged(changedDeviceStates);
			ZoneStateManager.ChangeOnDeviceState();
		}

		public void RemoveAllInitializing()
		{
			var changedDeviceStates = new List<DeviceState>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				device.DeviceState.States.RemoveAll(y => y.DriverState.Name == "Устройство инициализируется");
				changedDeviceStates.Add(device.DeviceState);
				device.DeviceState.OnStateChanged();
			}
			CallbackManager.DeviceStateChanged(changedDeviceStates);
			ZoneStateManager.ChangeOnDeviceState();
		}

		void SetStateByName(string stateName, Device device, ref List<DeviceState> changedDeviceStates)
		{
			var state = device.Driver.States.FirstOrDefault(y => y.Name == stateName);
			var deviceDriverState = new DeviceDriverState { DriverState = state, Time = DateTime.Now };
			device.DeviceState.States = new List<DeviceDriverState> { deviceDriverState };
			changedDeviceStates.Add(device.DeviceState);
			device.DeviceState.OnStateChanged();
		}
	}
}