using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public static class JournalHelper
	{
		static readonly object Locker = new object();
		static int _usbRequestNo;

		public static int GetLastSecJournalItemId2Op(Device device)
		{
			try
			{
				var lastindex = SendByteCommand(new List<byte> { 0x21, 0x02 }, device);
				return 256 * lastindex.Data[9] + lastindex.Data[10];
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetJournalCount(Device device)
		{
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return GetLastJournalItemId(device) + GetLastSecJournalItemId2Op(device);
			try
			{
				var firecount = SendByteCommand(new List<byte> { 0x24, 0x00 }, device);
				return 256 * firecount.Data[7] + firecount.Data[8];
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetFirstJournalItemId(Device device)
		{
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return 0;
			return GetLastJournalItemId(device) - GetJournalCount(device) + 1;
		}

		public static int GetLastJournalItemId(Device device)
		{
			try
			{
				var lastindex = SendByteCommand(new List<byte> { 0x21, 0x00 }, device);
				return 256 * lastindex.Data[9] + lastindex.Data[10];
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message + "\n Проверьте связь с прибором");
				throw;
			}
		}

		public static List<FSJournalItem> GetJournalItems(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<FSJournalItem>();
			for (int i = firstindex; i <= lastindex; i++)
				journalItems.Add(ReadItem(device, i));
			return journalItems;
		}

		public static List<FSJournalItem> GetSecJournalItems2Op(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<FSJournalItem>();
			for (int i = firstindex; i <= lastindex; i++)
				journalItems.Add(ReadSecItem(device, i));
			return journalItems;
		}

		public static List<FSJournalItem> GetAllJournalItems(Device device)
		{
			return GetJournalItems(device, GetLastJournalItemId(device), GetFirstJournalItemId(device));
		}

		private static FSJournalItem ReadItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			return SendBytesAndParse(bytes, device);
		}

		private static FSJournalItem ReadSecItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x02 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			return SendBytesAndParse(bytes, device);
		}

		private static FSJournalItem SendBytesAndParse(List<byte> bytes, Device device)
		{
			var data = SendByteCommand(bytes, device);
			//if (data != null && JournalParser.IsValidInput(data.Data))
			//{
			lock (Locker)
			{
				return JournalParser.FSParce(data.Data);
			}
			//}
			//else
			//{
			//    Trace.WriteLine("SendCode(bytes).Result.FirstOrDefault() == null");
			//    return new FSJournalItem();
			//}
		}

		private static ServerFS2.Response SendByteCommand(List<byte> commandBytes, Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(GetSheifByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			return ServerHelper.SendCode(bytes).Result.FirstOrDefault();
		}

		private static byte GetSheifByte(Device device)
		{
			//Convert.ToByte(device.AddressOnShleif);
			return 0x03;
		}
	}
}