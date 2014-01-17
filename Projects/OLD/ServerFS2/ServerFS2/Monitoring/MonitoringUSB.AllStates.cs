
namespace ServerFS2.Monitoring
{
	public partial class MonitoringUSB
	{
		public void SetAllInitializing(bool allDevices = true)
		{
			var device = this.USBDevice;
			if (allDevices)
				device = ConfigurationManager.DeviceConfiguration.RootDevice;

			device.DeviceState.IsInitializing = true;
			var deviceStatesManager = new DeviceStatesManager();
			deviceStatesManager.ForseUpdateDeviceStates(device);
		}

		public void RemoveAllInitializing(bool allDevices = true)
		{
			var device = this.USBDevice;
			if (allDevices)
				device = ConfigurationManager.DeviceConfiguration.RootDevice;

			device.DeviceState.IsInitializing = false;
			var deviceStatesManager = new DeviceStatesManager();
			deviceStatesManager.ForseUpdateDeviceStates(device);
		}
	}
}