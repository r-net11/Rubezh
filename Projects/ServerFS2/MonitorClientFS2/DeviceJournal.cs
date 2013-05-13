using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class DeviceJournal
	{
		static readonly object Locker = new object();
		int UsbRequestNo;

		public DeviceJournal()
		{
			;
		}

		public int GetLastSecJournalItemId2Op(Device device)
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

		public int GetJournalCount(Device device)
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

		public int GetFirstJournalItemId(Device device)
		{
			if (device.Driver.DriverType == DriverType.Rubezh_2OP)
				return 0;
			return GetLastJournalItemId(device) - GetJournalCount(device) + 1;
		}

		public int GetLastJournalItemId(Device device)
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

        public int GetLastJournalItemId2(Device device)
        {
            try
            {
                SendByteCommand2(new List<byte> { 0x21, 0x00 }, device);
            }
            catch (NullReferenceException ex)
            {
                MessageBox.Show(ex.Message + "\n Проверьте связь с прибором");
                throw;
            }
            return 0;
        }
        void SendByteCommand2(List<byte> commandBytes, Device device)
        {
            var bytes = new List<byte>();
            bytes.AddRange(BitConverter.GetBytes(++UsbRequestNo).Reverse());
            bytes.Add(GetSheifByte(device));
            bytes.Add(Convert.ToByte(device.AddressOnShleif));
            bytes.Add(0x01);
            bytes.AddRange(commandBytes);
            //lock (Locker)
            {
                ServerHelper.SendCodeAsync(bytes);
            }
        }


		public List<FSJournalItem> GetJournalItems(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<FSJournalItem>();
			for (int i = firstindex; i <= lastindex; i++)
				journalItems.Add(ReadItem(device, i));
			return journalItems;
		}

		public List<FSJournalItem> GetSecJournalItems2Op(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<FSJournalItem>();
			for (int i = firstindex; i <= lastindex; i++)
				journalItems.Add(ReadSecItem(device, i));
			return journalItems;
		}

		public List<FSJournalItem> GetAllJournalItems(Device device)
		{
			return GetJournalItems(device, GetLastJournalItemId(device), GetFirstJournalItemId(device));
		}

		FSJournalItem ReadItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			return SendBytesAndParse(bytes, device);
		}

		FSJournalItem ReadSecItem(Device device, int i)
		{
			List<byte> bytes = new List<byte> { 0x20, 0x02 };
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			return SendBytesAndParse(bytes, device);
		}

		FSJournalItem SendBytesAndParse(List<byte> bytes, Device device)
		{
			var data = SendByteCommand(bytes, device);
			lock (Locker)
			{
				return JournalParser.FSParce(data.Data);
			}
		}

		ServerFS2.Response SendByteCommand(List<byte> commandBytes, Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++UsbRequestNo).Reverse());
			bytes.Add(GetSheifByte(device));
			bytes.Add(Convert.ToByte(device.AddressOnShleif));
			bytes.Add(0x01);
			bytes.AddRange(commandBytes);
			lock (Locker)
			{
				return ServerHelper.SendCode(bytes).Result.FirstOrDefault();
			}
		}

		byte GetSheifByte(Device device)
		{
			//Convert.ToByte(device.AddressOnShleif);
			return 0x03;
		}
	}
}