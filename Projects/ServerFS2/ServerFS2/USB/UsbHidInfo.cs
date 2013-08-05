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
				var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1, "Запрос количества каналов МС");
				USBDriverType = DriversHelper.GetUsbDriverTypeByTypeNo(TypeNo);
				if (responce.Bytes[5] == 0x41)
					USBDriverType = DriverType.MS_2;
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
			var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1, "Запрос типа устройства");
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
			var responce = UsbHid.AddRequest(-1, bytesList, 1000, 1000, true, 1, "Установка идентификатора пакета");
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
			var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1, "Запрос наличия идентификатора пакета");
			return responce != null;
		}

		string GetUSBSerialNo()
		{
			var bytes = new List<byte>() { 0x01, 0x01, 0x32 };
			var bytesList = new List<List<byte>>();
			bytesList.Add(bytes);
			var responce = UsbHid.AddRequest(USBManager.NextRequestNo, bytesList, 1000, 1000, true, 1, "Запрос серийного номера");
			if (responce != null)
			{
				responce.Bytes.RemoveRange(0, 6);
				return BytesHelper.BytesToStringDescription(responce.Bytes);
			}
			return null;
		}

		public void WriteConfigToMS()
		{
			foreach (var chanel in USBDevice.Children)
			{
				var bytes = new List<byte>();
				var chanelAddressByte = Convert.ToByte(chanel.Driver.Properties.FirstOrDefault(x => x.Name == "Address").Default);
				var baudRateByte = Convert.ToByte(chanel.Parent.Driver.Properties.FirstOrDefault(x => x.Name == "BaudRate").Default);

				var chanelAddress = chanel.Properties.FirstOrDefault(x => x.Name == "Address");
				if (chanelAddress != null)
					chanelAddressByte = Convert.ToByte(chanelAddress.Value);
				var baudRate = chanel.Parent.Properties.FirstOrDefault(x => x.Name == "BaudRate");
				if (baudRate != null)
					baudRateByte = Convert.ToByte(baudRate.Value);
				bytes.Add(chanelAddressByte);
				foreach (var child in chanel.Children)
				{
					bytes.Add((byte)child.AddressOnShleif);
				}
				bytes.Sort();
				int nullCount = 32 - bytes.Count;
				for (int i = 0; i < nullCount; i++)
					bytes.Add(0x00);

				var allBytes = new List<byte>() { 0x01, 0x02, (byte)(chanel.IntAddress + 2), chanelAddressByte, baudRateByte };
				allBytes.AddRange(bytes);
				var responce = UsbHid.AddRequest(USBManager.NextRequestNo, new List<List<byte>>() { allBytes }, 1000, 1000, true, 1, "Запись конфигурации МС");
				//USBManager.Send(chanel.Parent, 0x02, chanel.IntAddress + 2, chanelAddress, baudRate, bytes);
			}
		}
	}
}