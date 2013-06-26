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
			ConfigurationManager.DeviceConfiguration.RootDevice.DeviceState.IsInitializing = true;
			var deviceStatesManager = new DeviceStatesManager();
			deviceStatesManager.ChangeDeviceStates(ConfigurationManager.DeviceConfiguration.RootDevice);
		}

		public void RemoveAllInitializing()
		{
			ConfigurationManager.DeviceConfiguration.RootDevice.DeviceState.IsInitializing = false;
			var deviceStatesManager = new DeviceStatesManager();
			deviceStatesManager.ChangeDeviceStates(ConfigurationManager.DeviceConfiguration.RootDevice);
		}
	}
}