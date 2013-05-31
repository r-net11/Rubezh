using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Device = FiresecAPI.Models.Device;
using FS2Api;
using System.Threading;
using ServerFS2.DataBase;

namespace ServerFS2
{
	public static partial class ServerHelper
	{
		public static FS2JournalItem ParseJournal(List<byte> bytes)
		{
			lock (Locker)
			{
				var journalParser = new JournalParser();
				var journalItem = journalParser.Parce(bytes);
				return journalItem;
			}
		}

		public static List<FS2JournalItem> GetSecJournalItems2Op(Device device)
		{
			int lastIndex = GetLastSecJournalItemId2Op(device);
			var journalItems = new List<FS2JournalItem>();
			for (int i = 0; i <= lastIndex; i++)
			{
				var bytes = new List<byte>();
				bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x01);
				bytes.Add(0x20);
				bytes.Add(0x02);
				bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				var result = SendCode(bytes).Result.FirstOrDefault();
				if (result != null)
				{
					var journalItem = ParseJournal(result.Data);
					journalItems.Add(journalItem);
				}
			}
			journalItems = journalItems.OrderByDescending(x => x.IntDate).ToList();
			int no = 0;
			foreach (var journalItem in journalItems)
			{
				no++;
				journalItem.No = no;
			}
			return journalItems;
		}

		public static int GetLastSecJournalItemId2Op(Device device)
		{
			var bytes = new List<byte>();
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x21);
			bytes.Add(0x02);
			try
			{
				var lastindex = SendCode(bytes);
				int li = 256 * 256 * 256 * lastindex.Result.FirstOrDefault().Data[7] + 256 * 256 * lastindex.Result.FirstOrDefault().Data[8] + 256 * lastindex.Result.FirstOrDefault().Data[9] + lastindex.Result.FirstOrDefault().Data[10];
				return li;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetJournalCount(Device device)
		{
			var bytes = new List<byte>();
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x24);
			bytes.Add(0x01);
			try
			{
				var firecount = SendCode(bytes);
				int fc = 256 * firecount.Result.FirstOrDefault().Data[7] + firecount.Result.FirstOrDefault().Data[8];
				return fc;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetFirstJournalItemId(Device device)
		{
			var li = GetLastJournalItemId(device);
			var count = GetJournalCount(device);
			return li - count + 1;
		}

		public static int GetLastJournalItemId(Device device)
		{
			Thread.Sleep(TimeSpan.FromSeconds(10));
			var bytes = new List<byte>();
			bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x21);
			bytes.Add(0x00);
			try
			{
				var lastindex = SendCode(bytes);
				int result = 256 * 256* 256* lastindex.Result.FirstOrDefault().Data[7] + 256* 256*lastindex.Result.FirstOrDefault().Data[8] + 256 * lastindex.Result.FirstOrDefault().Data[9] + lastindex.Result.FirstOrDefault().Data[10];
				return result;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static List<FS2JournalItem> GetJournalItems(Device device)
		{
			int lastindex = GetLastJournalItemId(device);
			int firstindex = GetFirstJournalItemId(device);
			var journalItems = new List<FS2JournalItem>();
			var secJournalItems = new List<FS2JournalItem>();
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
			{
				secJournalItems = GetSecJournalItems2Op(device);
			}
			//for (int i = firstindex; i <= lastindex; i++)
			for (int i = lastindex-10; i <= lastindex; i++)
			{
				var bytes = new List<byte>();
				bytes.Add(Convert.ToByte(device.Parent.IntAddress + 2));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x01);
				bytes.Add(0x20);
				bytes.Add(0x00);
				bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				var result = SendCode(bytes).Result.FirstOrDefault();
				if (result != null)
				{
					var journalItem = ParseJournal(result.Data);
					if (journalItem != null)
					{
						journalItems.Add(journalItem);
					}
				}
			}
			int no = 0;
			foreach (var item in journalItems)
			{
				no++;
				item.No = no;
			}
			secJournalItems.ForEach(x => journalItems.Add(x)); // в случае, если устройство не Рубеж-2ОП, коллекция охранных событий будет пустая
			return journalItems;
		}		
	}
}