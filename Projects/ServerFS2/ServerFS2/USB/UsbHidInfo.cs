using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Models;
using System;
using System.Diagnostics;

namespace ServerFS2
{
	public class UsbHidInfo
	{
		public UsbHid UsbHid { get; set; }
		public string SerialNo { get; set; }
		public int TypeNo { get; set; }
		public DriverType USBDriverType { get; set; }
		public Device USBDevice { get; set; }

		public void Initialize()
		{
			SetIdOn();
			if (UsbHid.UseId)
			{
				TypeNo = -1;
				var bytes = new List<byte>() { 0x01, 0x01, 0x04 };
				var bytesList = new List<List<byte>>();
				bytesList.Add(bytes);
				var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
				USBDriverType = DriversHelper.GetUsbDriverTypeByTypeNo(TypeNo);
				if (responce.Bytes[5] == 0x41)
					USBDriverType = DriverType.MS_2;
				else
					USBDriverType = DriverType.MS_1;
			}
			else
			{
				TypeNo = GetUSBTypeNo();
				USBDriverType = DriversHelper.GetUsbDriverTypeByTypeNo(TypeNo);
			}
			SerialNo = GetUSBSerialNo();
		}

		int GetUSBTypeNo()
		{
			var bytes = new List<byte>() { 0x02, 0x01, 0x03 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			if (responce != null)
			{
				if (responce.Bytes.Count > 2)
				{
					return responce.Bytes[2];
				}
			}
			return -1;
		}

		void SetIdOn()
		{
			//if (HasResponceWithoutID())
			//{
			//    UsbHid.UseId = false;
			//    var bytes = new List<byte>() { 0x01, 0x02, 0x34, 0x01 };
			//    var bytesList = new List<List<byte>>();
			//    bytesList.Add(bytes);
			//    var responce = UsbHid.AddRequest(-1, bytesList, 1000, 1000, true, 1);
			//    UsbHid.UseId = true;
			//}
			UsbHid.UseId = true;
			var result = HasResponceWithID();
			if (!result)
			{
				UsbHid.UseId = false;
				result = SetResponceId();
				UsbHid.UseId = true;
				result = HasResponceWithID();
			}
			UsbHid.UseId = result;
		}

		bool SetResponceId()
		{
			var bytes = new List<byte>() { 0x01, 0x02, 0x34, 0x01 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbHid.AddRequest(-1, bytesList, 1000, 1000, true, 1);
			return responce != null;
		}
		//bool HasResponceWithoutID()
		//{
		//    var bytes = new List<byte>() { 0x01, 0x01, 0x34 };
		//    var bytesList = new List<List<byte>>();
		//    bytesList.Add(bytes);
		//    var responce = UsbHid.AddRequest(-1, bytesList, 1000, 1000, true, 1);
		//    return responce != null;
		//}

		bool HasResponceWithID()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x34 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			return responce != null;
		}

		string GetUSBSerialNo()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x32 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			if (responce != null)
			{
				responce.Bytes.RemoveRange(0, 6);
				return BytesHelper.BytesToStringDescription(responce.Bytes);
			}
			return null;
		}

		public void WriteConfigToMS()
		{
			foreach (var channelDevice in USBDevice.Children)
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
				for (int i = 1; i <= 256; i++)
				{

					if (!channelDevice.Children.Any(x => x.IntAddress == i))
					{
						freeChannelAddress = (byte)i;
						break;
					}
				}

				byte baudRateValue = 0;
				var baudRateProperty = USBDevice.Properties.FirstOrDefault(x => x.Name == "BaudRate");
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
				var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1);
			}
		}
	}
}