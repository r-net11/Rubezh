using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2
{
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
			USBDriverType = DriversHelper.GetUsbDriverTypeByTypeNo(usbTypeNo);
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
			var result = UsbRunner.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			var responce = result.Result.FirstOrDefault();
			return responce != null;
		}

		string GetUSBSerialNo()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x32 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var result = UsbRunner.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
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