using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Models;
using System.Diagnostics;
using ServerFS2.ConfigurationWriter;

namespace ServerFS2
{
	public static class USBManager
	{
		public static int RequestNo = 0;

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

		public static DriverType GetTypeNo(int driverTypeNo)
		{
			switch (driverTypeNo)
			{
				case 1:
					return DriverType.USB_Rubezh_2AM;
					//return DriverType.Rubezh_2AM;
				case 6:
					return DriverType.USB_Rubezh_2OP;
					//return DriverType.Rubezh_2OP;
				case 5:
					return DriverType.USB_Rubezh_4A;
					//return DriverType.Rubezh_4A;
				case 2:
					return DriverType.USB_BUNS;
					//return DriverType.BUNS;
				case 8:
					return DriverType.USB_BUNS_2;
					//return DriverType.BUNS_2;
				case 3:
					return DriverType.IndicationBlock;
				case 7:
					return DriverType.PDU;
				case 9:
					return DriverType.PDU_PT;
				case 10:
					return DriverType.USB_Rubezh_P;
					//return DriverType.BlindPanel;
				case 4:
					return DriverType.Rubezh_10AM;
				case 98:
					return DriverType.MS_1;
				case 99:
					return DriverType.MS_2;
				case 100:
					return DriverType.MS_3;
				case 101:
					return DriverType.MS_4;
				case 102:
					return DriverType.UOO_TL;
			}
			return DriverType.Computer;
		}
	}

	public class UsbRunnerDetector
	{
		public UsbRunner2 UsbRunner { get; set; }
		public string SerialNo { get; set; }
		public int TypeNo { get; set; }
		public bool IsUSBPanel { get; set; }
		public bool IsUSBMS { get; set; }
		public DriverType USBDriverType { get; set; }
		public Device Device { get; set; }

		public void Initialize()
		{
			var usbTypeNo = GetUSBTypeNo();
			USBDriverType = USBManager.GetTypeNo(usbTypeNo);
			if (usbTypeNo == -1)
			{
				IsUSBMS = true;
				SetIdOn();
				UsbRunner.IsUsbDevice = false;
			}
			else
			{
				IsUSBPanel = true;
				UsbRunner.IsUsbDevice = true;
			}

			SerialNo = GetUSBSerialNo();
		}

		int GetUSBTypeNo()
		{
			UsbRunner.IsUsbDevice = true;
			var bytes = new List<byte>() { 0x02, 0x01, 0x03 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var result = UsbRunner.AddRequest(-1, bytesList, 1000, 1000, true, 1);
			var responce = result.Result.FirstOrDefault();
			if (responce != null)
			{
				if (responce.Data.Count > 2)
				{
					return responce.Data[2];
				}
			}
			UsbRunner.IsUsbDevice = false;
			return -1;
		}

		bool SetIdOn()
		{
			if (HasResponceWithoutID())
			{
				UsbRunner.IsUsbDevice = true;
				var bytes = new List<byte>() { 0x01, 0x02, 0x34, 0x01 };
				var bytesList = new List<List<byte>>();
				bytesList.Add(bytes);
				var result = UsbRunner.AddRequest(-1, bytesList, 1000, 1000, true, 1);
				UsbRunner.IsUsbDevice = false;
				var responce = result.Result.FirstOrDefault();
			}
			return HasResponceWithID();
		}

		bool HasResponceWithoutID()
		{
			UsbRunner.IsUsbDevice = true;
			var bytes = new List<byte>() { 0x01, 0x01, 0x34 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var result = UsbRunner.AddRequest(-1, bytesList, 1000, 1000, true, 1);
			UsbRunner.IsUsbDevice = false;
			var responce = result.Result.FirstOrDefault();
			return responce != null;
		}

		bool HasResponceWithID()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x34 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var result = UsbRunner.AddRequest(USBManager.RequestNo++, bytesList, 1000, 1000, true, 1);
			var responce = result.Result.FirstOrDefault();
			return responce != null;
		}

		string GetUSBSerialNo()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x32 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var result = UsbRunner.AddRequest(USBManager.RequestNo++, bytesList, 1000, 1000, true, 1);
			var responce = result.Result.FirstOrDefault();
			if (responce != null)
			{
				responce.Data.RemoveRange(0, 6);
				return BytesHelper.BytesToStringDescription(responce.Data);
			}
			return null;
		}
	}
}