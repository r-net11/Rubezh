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
		public static List<UsbHidInfo> Detect()
		{
			var usbDevices = GetAllUSBDevices();
			var usbHidInfos = FindAllUsbHidInfo();

			foreach (var device in usbDevices)
			{
				UsbHidInfo usbHidInfo = null;
				var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (serialNoProperty != null && !string.IsNullOrEmpty(serialNoProperty.Value))
				{
					usbHidInfo = usbHidInfos.FirstOrDefault(x => x.SerialNo == serialNoProperty.Value);
					if (usbHidInfo != null)
					{
						usbHidInfo.USBDevice = device;
						continue;
					}
				}

				var driverTypeNo = DriversHelper.GetTypeNoByDriverType(device.Driver.DriverType);
				usbHidInfo = usbHidInfos.FirstOrDefault(x => x.TypeNo == driverTypeNo);
				if (usbHidInfo != null)
				{
					usbHidInfo.USBDevice = device;
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
						var usbHidInfo = usbHidInfos.FirstOrDefault(x => x.TypeNo == -1 && x.USBDevice == null);
						if (usbHidInfo != null)
						{
							usbHidInfo.USBDevice = device;
						}
					}
				}
			}

			foreach (var usbHidInfo in usbHidInfos)
			{
				if (usbHidInfo.USBDevice == null)
					usbHidInfo.UsbHid.Dispose();
			}
			usbHidInfos.RemoveAll(x => x.USBDevice == null);

			foreach (var usbHidInfo in usbHidInfos)
			{
				switch (usbHidInfo.USBDevice.Driver.DriverType)
				{
					case DriverType.MS_1:
					case DriverType.MS_2:
						//usbHidInfo.WriteConfigToMS();
						break;
				}
			}

			return usbHidInfos;
		}

		public static List<UsbHidInfo> FindAllUsbHidInfo()
		{
			try
			{
				var usbHidInfos = new List<UsbHidInfo>();

				var usbHidPorts = USBDeviceFinder.FindDevices(0xC251, 0x1303);
				foreach (var usbHidPort in usbHidPorts)
				{
					var usbHid = new UsbHid();
					usbHid.SetUsbHidPort(usbHidPort);
					var usbHidInfo = new UsbHidInfo()
					{
						UsbHid = usbHid
					};
					usbHidInfos.Add(usbHidInfo);
				}

				foreach (var usbHidInfo in usbHidInfos)
				{
					usbHidInfo.Initialize();
				}
				return usbHidInfos;
			}
			catch (Exception e)
			{
				return new List<UsbHidInfo>();
			}
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
						usbDevices.Add(device);
						break;
				}
			}
			return usbDevices;
		}
	}
}