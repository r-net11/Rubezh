using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace FiresecService
{
	public partial class FiresecManager
	{
		public void ConvertStates()
		{
			DeviceConfigurationStates = new DeviceConfigurationStates();
			if (ConfigurationManager.DeviceConfiguration.Devices.IsNotNullOrEmpty())
			{
				foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
				{
					var deviceState = new DeviceState()
					{
						UID = device.UID,
						PlaceInTree = device.PlaceInTree,
						Device = device
					};

					foreach (var parameter in device.Driver.Parameters)
						deviceState.Parameters.Add(parameter.Copy());

					DeviceConfigurationStates.DeviceStates.Add(deviceState);
				}
			}

			foreach (var zone in ConfigurationManager.DeviceConfiguration.Zones)
			{
				DeviceConfigurationStates.ZoneStates.Add(new ZoneState() { No = zone.No });
			}
		}
	}
}