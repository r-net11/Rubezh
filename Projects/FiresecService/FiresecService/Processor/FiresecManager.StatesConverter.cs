using FiresecAPI.Models;
using FiresecService.Service;

namespace FiresecService.Processor
{
	public partial class FiresecManager
	{
		public void ConvertStates()
		{
			DeviceConfigurationStates = new DeviceConfigurationStates();
			if (ConfigurationCash.DeviceConfiguration.Devices.IsNotNullOrEmpty())
			{
				foreach (var device in ConfigurationCash.DeviceConfiguration.Devices)
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

			foreach (var zone in ConfigurationCash.DeviceConfiguration.Zones)
			{
				DeviceConfigurationStates.ZoneStates.Add(new ZoneState() { No = zone.No });
			}
		}
	}
}