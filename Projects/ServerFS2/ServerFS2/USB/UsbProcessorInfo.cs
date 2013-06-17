using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;

namespace ServerFS2
{
	public class UsbProcessorInfo
	{
		public UsbProcessor UsbProcessor { get; set; }
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
				UsbProcessor.IsUsbDevice = false;
			}
			else
			{
				IsUSBPanel = true;
				UsbProcessor.IsUsbDevice = true;
			}

			SerialNo = GetUSBSerialNo();
		}

		int GetUSBTypeNo()
		{
			UsbProcessor.IsUsbDevice = true;
			var bytes = new List<byte>() { 0x02, 0x01, 0x03 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbProcessor.AddRequest(-1, bytesList, 1000, 1000, true, 1);
			if (responce != null)
			{
				if (responce.Bytes.Count > 2)
				{
					return responce.Bytes[2];
				}
			}
			UsbProcessor.IsUsbDevice = false;
			return -1;
		}

		bool SetIdOn()
		{
			if (HasResponceWithoutID())
			{
				UsbProcessor.IsUsbDevice = true;
				var bytes = new List<byte>() { 0x01, 0x02, 0x34, 0x01 };
				var bytesList = new List<List<byte>>();
				bytesList.Add(bytes);
				var responce = UsbProcessor.AddRequest(-1, bytesList, 1000, 1000, true, 1);
				UsbProcessor.IsUsbDevice = false;
			}
			return HasResponceWithID();
		}

		bool HasResponceWithoutID()
		{
			UsbProcessor.IsUsbDevice = true;
			var bytes = new List<byte>() { 0x01, 0x01, 0x34 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbProcessor.AddRequest(-1, bytesList, 1000, 1000, true, 1);
			UsbProcessor.IsUsbDevice = false;
			return responce != null;
		}

		bool HasResponceWithID()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x34 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbProcessor.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			return responce != null;
		}

		string GetUSBSerialNo()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x32 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbProcessor.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			if (responce != null)
			{
				responce.Bytes.RemoveRange(0, 6);
				return BytesHelper.BytesToStringDescription(responce.Bytes);
			}
			return null;
		}
	}
}