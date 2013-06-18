using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using System;

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
			TypeNo = GetUSBTypeNo();
			USBDriverType = DriversHelper.GetUsbDriverTypeByTypeNo(TypeNo);
			if (TypeNo == -1)
			{
				IsUSBMS = true;
				SetIdOn();
				UsbProcessor.WithoutId = false;
			}
			else
			{
				IsUSBPanel = true;
				UsbProcessor.WithoutId = true;
			}

			SerialNo = GetUSBSerialNo();
		}

		int GetUSBTypeNo()
		{
			UsbProcessor.WithoutId = true;
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
			UsbProcessor.WithoutId = false;
			return -1;
		}

		bool SetIdOn()
		{
			if (HasResponceWithoutID())
			{
				UsbProcessor.WithoutId = true;
				var bytes = new List<byte>() { 0x01, 0x02, 0x34, 0x01 };
				var bytesList = new List<List<byte>>();
				bytesList.Add(bytes);
				var responce = UsbProcessor.AddRequest(-1, bytesList, 1000, 1000, true, 1);
				UsbProcessor.WithoutId = false;
			}
			return HasResponceWithID();
		}

		bool HasResponceWithoutID()
		{
			UsbProcessor.WithoutId = true;
			var bytes = new List<byte>() { 0x01, 0x01, 0x34 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbProcessor.AddRequest(-1, bytesList, 1000, 1000, true, 1);
			UsbProcessor.WithoutId = false;
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

		public void WriteConfigToMS()
		{
			foreach (var channelDevice in Device.Children)
			{
				byte paramNo = 0;
				switch (channelDevice.IntAddress)
				{
					case 1:
						paramNo = 0x03;
						break;

					case 2:
						paramNo = 0x04;
						break;
				}

				byte freeChannelAddress = 0;
				for (int i = 1; i <= 100; i++)
				{

					if (!channelDevice.Children.Any(x => x.IntAddress == i))
					{
						freeChannelAddress = (byte)i;
						break;
					}
				}

				byte baudRateValue = 0;
				var baudRateProperty = Device.Properties.FirstOrDefault(x => x.Name == "BaudRate");
				if (baudRateProperty != null)
				{
					baudRateValue = Byte.Parse(baudRateProperty.Value);
				}

				var addressListBytes = new List<byte>();
				addressListBytes.Add(freeChannelAddress);
				var bytesToAdd = 32 - addressListBytes.Count();
				for (int i = 0; i < bytesToAdd; i++)
				{
					addressListBytes.Add(0);
				}

				var bytes = new List<byte>() { 0x01, 0x02, paramNo, freeChannelAddress, baudRateValue };
				bytes.AddRange(addressListBytes);
				var bytesList = new List<List<byte>>();
				bytesList.Add(bytes);
				var responce = UsbProcessor.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			}
		}
	}
}