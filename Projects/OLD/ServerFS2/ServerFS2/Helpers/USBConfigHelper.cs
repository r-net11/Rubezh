using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static class USBConfigHelper
	{
		static DeviceConfiguration CurrentDeviceConfiguration;

		public static Device SetTempDeviceConfiguration(Device device)
		{
			CurrentDeviceConfiguration = ConfigurationManager.DeviceConfiguration;
			ConfigurationManager.DeviceConfiguration = CreateTempDeviceConfiguration(device);
			USBManager.Initialize();
			return ConfigurationManager.DeviceConfiguration.RootDevice.Children.FirstOrDefault();
		}

		public static void SetCurrentDeviceConfiguration()
		{
			ConfigurationManager.DeviceConfiguration = CurrentDeviceConfiguration;
			USBManager.Initialize();
		}

		public static DeviceConfiguration CreateTempDeviceConfiguration(Device device)
		{
			var deviceConfiguration = new DeviceConfiguration();
			deviceConfiguration.RootDevice = new Device();
			deviceConfiguration.RootDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == DriverType.Computer);
			deviceConfiguration.RootDevice.DriverUID = deviceConfiguration.RootDevice.Driver.UID;

			Device usbDevice = (Device)device.Clone();
			var driverType = DriverTypeToUSBDriverType(device.Driver.DriverType);

			usbDevice.Driver = ConfigurationManager.Drivers.FirstOrDefault(x => x.DriverType == driverType);
			usbDevice.DriverUID = deviceConfiguration.RootDevice.Driver.UID;

			deviceConfiguration.RootDevice.Children.Add(usbDevice);
			usbDevice.Parent = deviceConfiguration.RootDevice;
			return deviceConfiguration;
		}

		static DriverType DriverTypeToUSBDriverType(DriverType driverType)
		{
			switch (driverType)
			{
				case DriverType.Rubezh_2AM:
					return DriverType.USB_Rubezh_2AM;

				case DriverType.Rubezh_2OP:
					return DriverType.USB_Rubezh_2OP;

				case DriverType.Rubezh_4A:
					return DriverType.USB_Rubezh_4A;

				case DriverType.BUNS:
					return DriverType.USB_BUNS;

				case DriverType.Rubezh_P:
					return DriverType.USB_Rubezh_P;
			}
			return DriverType.Computer;
		}
	}
}