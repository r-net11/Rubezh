using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI;
using FiresecAPI.Models;

namespace ServerFS2
{
	public static class USBManager
	{
		static int RequestNo = 0;
		public static int NextRequestNo
		{
			get { return RequestNo++; }
		}

		public static readonly UsbRunnerBase UsbRunnerBase;

		static USBManager()
		{
			UsbRunnerBase = new UsbRunner2();
			try
			{
				UsbRunnerBase.Open();
			}
			catch
			{ }
		}

		public static List<byte> SendCodeToPanel(List<byte> bytes, Device device, int maxDelay = 1000, int maxTimeout = 1000)
		{
			bytes.InsertRange(0, IsUsbDevice ? new List<byte> { (byte)(0x02) } : new List<byte> { (byte)(device.Parent.IntAddress + 2), (byte)device.IntAddress });
			var result = UsbRunnerBase.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, true).Result[0].Data;
			result.RemoveRange(0, IsUsbDevice ? 2 : 7);
			return result;
		}

		public static List<byte> SendCodeToPanel(Device device, params object[] value)
		{
			var bytes = CreateBytesArray(value);
			bytes.InsertRange(0, IsUsbDevice ? new List<byte> { (byte)(0x02) } : new List<byte> { (byte)(device.Parent.IntAddress + 2), (byte)device.IntAddress });
			var result = UsbRunnerBase.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, 1000, 1000, true);
			if (result != null)
			{
				var responce = result.Result.FirstOrDefault();
				if (responce != null)
				{
					var data = responce.Data;
					data.RemoveRange(0, IsUsbDevice ? 2 : 7);
					return data;
				}
			}
			return null;
		}

		public static OperationResult<List<Response>> SendCode(List<List<byte>> bytesList, int maxDelay = 1000, int maxTimeout = 1000)
		{
			return UsbRunnerBase.AddRequest(NextRequestNo, bytesList, maxDelay, maxTimeout, true);
		}

		public static List<byte> SendCode(List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
		{
			return UsbRunnerBase.AddRequest(NextRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, true).Result[0].Data;
		}

		public static void SendCodeAsync(int usbRequestNo, List<byte> bytes, int maxDelay = 1000, int maxTimeout = 1000)
		{
			UsbRunnerBase.AddRequest(usbRequestNo, new List<List<byte>> { bytes }, maxDelay, maxTimeout, false);
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

		public static bool IsUsbDevice
		{
			get { return UsbRunnerBase.IsUsbDevice; }
			set
			{
				UsbRunnerBase.IsUsbDevice = value;
				UsbRunnerBase.Close();
				UsbRunnerBase.Open();
			}
		}

		public static List<byte> SendRequest(List<byte> bytes)
		{
			return SendCode(bytes);
		}

		public static void Initialize()
		{
			var usbDevices = new List<Device>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.RootDevice.Children)
			{
				switch (device.Driver.DriverType)
				{
					case FiresecAPI.Models.DriverType.MS_1:
					case FiresecAPI.Models.DriverType.MS_2:
						usbDevices.Add(device);
						break;
				}
			}

			var usbRunnerDetectors = new List<UsbRunnerDetector>();

			while (true)
			{
				try
				{
					var usbRunner = new UsbRunner2();
					var result = usbRunner.Open();
					if (!result)
						break;
					var usbRunnerDetector = new UsbRunnerDetector()
					{
						UsbRunner = usbRunner
					};
					usbRunnerDetectors.Add(usbRunnerDetector);
				}
				catch (Exception)
				{
					break;
				}
			}

			foreach (var usbRunnerDetector in usbRunnerDetectors)
			{
				usbRunnerDetector.Initialize();
				Trace.WriteLine(usbRunnerDetector.IsUSBMS + " " + usbRunnerDetector.IsUSBPanel + " " +
					usbRunnerDetector.USBDriverType + " " + usbRunnerDetector.SerialNo);
			}

			foreach (var device in usbDevices)
			{
				var serialNoProperty = device.Properties.FirstOrDefault(x => x.Name == "SerialNo");
				if (serialNoProperty != null)
				{
					var usbRunnerDetector = usbRunnerDetectors.FirstOrDefault(x => x.SerialNo == serialNoProperty.Value);
					if (usbRunnerDetector != null)
					{
						usbRunnerDetector.Device = device;
					}
				}
				else
				{
				}
			}
		}
	}
}