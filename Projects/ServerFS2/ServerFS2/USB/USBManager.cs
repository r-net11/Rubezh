using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;
using UsbLibrary;

namespace ServerFS2
{
	public static class USBManager
	{
		static int RequestNo = 0;
		public static int NextRequestNo
		{
			get { return RequestNo++; }
		}

		public static List<UsbProcessorInfo> UsbProcessorInfos { get; set; }

		static USBManager()
		{
			Initialize();
		}

		public static List<byte> SendCodeToPanel(Device device, params object[] value)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				var bytes = CreateBytesArray(value);
				bytes.InsertRange(0, usbProcessor.IsUsbDevice ? new List<byte> { (byte)(0x02) } : new List<byte> { (byte)(device.Parent.IntAddress + 2), (byte)device.IntAddress });
				var responce = usbProcessor.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, 1000, 1000, true);
				if (responce != null)
				{
					var data = responce.Bytes;
					data.RemoveRange(0, usbProcessor.IsUsbDevice ? 2 : 7);
					return data;
				}
				return null;
			}
			else
			{
				return null;
			}
		}

		public static int SendCodeToPanelAsync(Device device, params object[] value)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				var bytes = CreateBytesArray(value);
				bytes.InsertRange(0, usbProcessor.IsUsbDevice ? new List<byte> { (byte)(0x02) } : new List<byte> { (byte)(device.Parent.IntAddress + 2), (byte)device.IntAddress });
				var requestNo = NextRequestNo;
				usbProcessor.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, 1000, 1000, false);
				return requestNo;
			}
			else
			{
				return -1;
			}
		}

		public static bool IsUsbDevice(Device device)
		{
			var usbProcessor = GetUsbProcessor(device);
			if (usbProcessor != null)
			{
				return usbProcessor.IsUsbDevice;
			}
			return false;
		}

		public static UsbProcessor GetUsbProcessor(Device panelDevice)
		{
			var parentUSB = panelDevice.ParentUSB;
			if (parentUSB != null)
			{
				var usbProcessorInfo = UsbProcessorInfos.FirstOrDefault(x => x.Device.UID == parentUSB.UID);
				if (usbProcessorInfo != null)
				{
					return usbProcessorInfo.UsbProcessor;
				}
			}
			return null;
		}

		public static List<byte> CreateBytesArray(params object[] values)
		{
			var bytes = new List<byte>();
			foreach (var value in values)
			{
				if (value as IEnumerable<Byte> != null)
					bytes.AddRange((IEnumerable<Byte>)value);
				else
					bytes.Add(Convert.ToByte(value));
			}
			return bytes;
		}

		public static void Initialize()
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

			UsbProcessorInfos = new List<UsbProcessorInfo>();

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
					UsbProcessorInfos.Add(usbProcessorInfo);
				}
				catch (Exception)
				{
					break;
				}
			}

			foreach (var usbRunnerDetector in UsbProcessorInfos)
			{
				usbRunnerDetector.Initialize();
				Trace.WriteLine(usbRunnerDetector.IsUSBMS + " " + usbRunnerDetector.IsUSBPanel + " " +
					usbRunnerDetector.USBDriverType + " " + usbRunnerDetector.SerialNo);
			}

			foreach (var device in usbDevices)
			{
				var driverTypeNo = DriversHelper.GetTypeNoByDriverType(device.Driver.DriverType);
				var usbRunnerDetector = UsbProcessorInfos.FirstOrDefault(x => x.TypeNo == driverTypeNo);
				if (usbRunnerDetector != null)
				{
					usbRunnerDetector.Device = device;
				}

				var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (serialNoProperty != null)
				{
					usbRunnerDetector = UsbProcessorInfos.FirstOrDefault(x => x.SerialNo == serialNoProperty.Value);
					if (usbRunnerDetector != null)
					{
						usbRunnerDetector.Device = device;
					}
				}
			}

			foreach (var usbRunnerDetector in UsbProcessorInfos)
			{
				if (usbRunnerDetector.Device == null)
					usbRunnerDetector.UsbProcessor.Close();
			}
			UsbProcessorInfos.RemoveAll(x => x.Device == null);
		}
	}
}