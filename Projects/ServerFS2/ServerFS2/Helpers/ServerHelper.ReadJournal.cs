using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using FS2Api;
using ServerFS2.ConfigurationWriter;
using Device = FiresecAPI.Models.Device;
using ServerFS2.Service;

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
				var bytes = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x01, 0x20, 0x02, BitConverter.GetBytes(i).Reverse());
				var result = SendCode(bytes);
				if (result == null) continue;
				var journalItem = ParseJournal(result);
				journalItems.Add(journalItem);
			}
			journalItems = journalItems.OrderByDescending(x => x.IntDate).ToList();
			var no = 0;
			foreach (var journalItem in journalItems)
			{
				no++;
				journalItem.No = no;
			}
			return journalItems;
		}

		public static int GetLastSecJournalItemId2Op(Device device)
		{
			var bytes = CreateBytesArray(0x01, 0x21, 0x02);
			try
			{
				var lastindex = SendCodeToPanel(bytes, device);
				return BytesHelper.ExtractInt(lastindex, 0);
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static int GetJournalCount(Device device)
		{
			var bytes = CreateBytesArray(0x01, 0x24, 0x01);
			try
			{
				var firecount = SendCodeToPanel(bytes, device);
				return BytesHelper.ExtractShort(firecount, 0);
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
			var bytes = CreateBytesArray(0x01, 0x21, 0x00);
			try
			{
				var lastindex = SendCodeToPanel(bytes, device);
				return BytesHelper.ExtractInt(lastindex, 0);
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static List<FS2JournalItem> GetJournalItems(Device device)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса последней записи"));
			int lastIndex = GetLastJournalItemId(device);
			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса первой записи"));
			int firstIndex = GetFirstJournalItemId(device);
			var journalItems = new List<FS2JournalItem>();
			var secJournalItems = new List<FS2JournalItem>();
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
			{
				secJournalItems = GetSecJournalItems2Op(device);
			}
			for (int i = firstIndex; i <= lastIndex; i++)
			//for (int i = lastindex-10; i <= lastindex; i++)
			{
				CallbackManager.AddProgress(new FS2ProgressInfo("Чтение записей журнала",  (i-firstIndex)*100/(lastIndex - firstIndex)));
				var bytes = CreateBytesArray(device.Parent.IntAddress + 2, device.IntAddress, 0x01, 0x20, 0x00, BitConverter.GetBytes(i).Reverse());
				var result = SendCode(bytes);
				if (result == null) continue;
				var journalItem = ParseJournal(result);
				if (journalItem != null)
				{
					journalItems.Add(journalItem);
				}
			}
			int no = 0;
			foreach (var item in journalItems)
			{
				no++;
				item.No = no;
			}
			secJournalItems.ForEach(journalItems.Add); // в случае, если устройство не Рубеж-2ОП, коллекция охранных событий будет пустая
			return journalItems;
		}		
	}
}