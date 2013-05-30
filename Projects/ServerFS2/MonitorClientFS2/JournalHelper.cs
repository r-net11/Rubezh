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
				var lastindex = SendByteCommandSync(new List<byte> { 0x21, 0x02 }, device);
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
				var firecount = SendByteCommandSync(new List<byte> { 0x24, 0x00 }, device);
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
				var response = SendByteCommandSync(new List<byte> { 0x21, 0x00 }, device);
				var result = 256 * 256 * 256 * response.Data[7] + 256 * 256 * response.Data[8] + 256 * response.Data[9] + response.Data[10];
				return result;
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
			{
				var journalItem = ReadItem(device, i);
				journalItems.Add(journalItem);
				MonitoringDevice.OnNewJournalItem(journalItem);
			}
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

		public static FSJournalItem ReadItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			for (int j = 0; j < 15; j++)
			{
				var fsJournalItem = SendBytesAndParse(bytes, device);
				if (fsJournalItem != null)
					return fsJournalItem; 
			}
			return null;
		}

		private static FSJournalItem ReadSecItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x02 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			return SendBytesAndParse(bytes, device);
		}

		private static FSJournalItem SendBytesAndParse(List<byte> bytes, Device device)
		{
			var response = SendByteCommand(bytes, device);
			if (response == null)
				return null;
			lock (Locker)
			{
				return JournalParser.FSParce(response.Data);
			}
		}

		static ServerFS2.Response SendByteCommand(List<byte> commandBytes, Device device)
		{
			var bytes = new List<byte>();
			bytes.Add(GetMSChannelByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			lock (Locker)
			{
				return ServerHelper.SendCode(bytes).Result.FirstOrDefault();
			}
		}

		private static ServerFS2.Response SendByteCommandSync(List<byte> commandBytes, Device device)
		{
			var bytes = new List<byte>();
			bytes.Add(GetMSChannelByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			return ServerHelper.SendCode(bytes).Result.FirstOrDefault();
		}

		public static void SendByteCommand(List<byte> commandBytes, Device device, int requestId)
		{
			var bytes = new List<byte>();
			bytes.Add(GetMSChannelByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			ServerHelper.SendCodeAsync(requestId, bytes);
		}

		private static byte GetMSChannelByte(Device device)
		{
			return 0x03;
		}
	}
}