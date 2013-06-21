using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using UsbLibrary;
using System.Diagnostics;

namespace ServerFS2
{
	public static class USBDetectorHelper
	{
		public static List<UsbProcessorInfo> Detect()
		{
			var usbDevices = GetAllUSBDevices();
			var usbProcessorInfos = FindAllUsbProcessorInfo();

			foreach (var device in usbDevices)
			{
				UsbProcessorInfo usbProcessorInfo = null;
				var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (serialNoProperty != null && !string.IsNullOrEmpty(serialNoProperty.Value))
				{
					usbProcessorInfo = usbProcessorInfos.FirstOrDefault(x => x.SerialNo == serialNoProperty.Value);
					if (usbProcessorInfo != null)
					{
						usbProcessorInfo.Device = device;
						continue;
					}
				}

				var driverTypeNo = DriversHelper.GetTypeNoByDriverType(device.Driver.DriverType);
				usbProcessorInfo = usbProcessorInfos.FirstOrDefault(x => x.TypeNo == driverTypeNo);
				if (usbProcessorInfo != null)
				{
					usbProcessorInfo.Device = device;
					continue;
				}
			}

			foreach (var device in usbDevices)
			{
				if (device.Driver.DriverType == DriverType.MS_1 || device.Driver.DriverType == DriverType.MS_2)
				{
					var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
					if (serialNoProperty == null || string.IsNullOrEmpty(serialNoProperty.Value))
					{
						var usbProcessorInfo = usbProcessorInfos.FirstOrDefault(x => x.TypeNo == -1 && x.Device == null);
						if (usbProcessorInfo != null)
						{
							usbProcessorInfo.Device = device;
						}
					}
				}
			}

			foreach (var usbProcessorInfo in usbProcessorInfos)
			{
				if (usbProcessorInfo.Device == null)
					usbProcessorInfo.UsbProcessor.Dispose();
			}
			usbProcessorInfos.RemoveAll(x => x.Device == null);

			foreach (var usbProcessorInfo in usbProcessorInfos)
			{
				switch (usbProcessorInfo.Device.Driver.DriverType)
				{
					case DriverType.MS_1:
					case DriverType.MS_2:
						usbProcessorInfo.WriteConfigToMS();
						break;
				}
			}

			return usbProcessorInfos;
		}

		public static List<UsbProcessorInfo> FindAllUsbProcessorInfo()
		{
			var usbProcessorInfos = new List<UsbProcessorInfo>();

			HIDDevice.AddedDevices = new List<string>();
			while (true)
			{
				try
				{
					var usbProcessor = new UsbProcessor();
					var result = usbProcessor.Open();
					if (!result)
						break;
					var usbProcessorInfo = new UsbProcessorInfo()
					{
						UsbProcessor = usbProcessor
					};
					usbProcessorInfos.Add(usbProcessorInfo);
				}
				catch (Exception)
				{
					break;
				}
			}

			foreach (var usbProcessorInfo in usbProcessorInfos)
			{
				usbProcessorInfo.Initialize();
			}
			return usbProcessorInfos;
		}

		static List<Device> GetAllUSBDevices()
		{
			var usbDevices = new List<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.RootDevice.Children)
			{
				switch (device.Driver.DriverType)
				{
					case DriverType.MS_1:
					case DriverType.MS_2:
					case DriverType.USB_Rubezh_2AM:
					case DriverType.USB_Rubezh_2OP:
					case DriverType.USB_Rubezh_4A:
					case DriverType.USB_Rubezh_P:
					case DriverType.USB_BUNS:
					case DriverType.USB_BUNS_2:
						usbDevices.Add(device);
						break;
				}
			}
			return usbDevices;
		}
	}
}