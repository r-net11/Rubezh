using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static class USBManager
	{
		public static void Initialize()
		{
			var usbDevices = new List<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.RootDevice.Children)
			{
				switch(device.Driver.DriverType)
				{
					case FiresecAPI.Models.DriverType.MS_1:
					case FiresecAPI.Models.DriverType.MS_2:
						usbDevices.Add(device);
						break;
				}
			}

			var usbRunners = new List<UsbRunner2>();

			while (true)
			{
				try
				{
					var usbRunnerBase = new UsbRunner2();
					usbRunnerBase.Open();
					usbRunners.Add(usbRunnerBase);
				}
				catch (Exception)
				{
					break;
				}
			}
		}
	}
}