using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using FiresecAPI;
using FiresecAPI.Models;
using ServerFS2;

namespace MonitorClientFS2
{
	public static class JournalHelper
	{
		static readonly object Locker = new object();
		static readonly UsbRunner UsbRunner;
		static readonly List<UsbRequest> UsbRequests = new List<UsbRequest>();

		static JournalHelper()
		{
			MetadataHelper.Initialize();
			UsbRunner = new UsbRunner();
			UsbRunner.Open();
		}

		private static int _usbRequestNo;

		public static void ParseJournal(List<byte> bytes, List<JournalItem> journalItems)
		{
			lock (Locker)
			{
				var journalParser = new JournalParser(bytes);
				var journalItem = journalParser.Parce();
				journalItems.Add(journalItem);
			}
		}

		private static OperationResult<List<Response>> SendCode(List<byte> bytes)
		{
			var usbRequest = new UsbRequest()
			{
				Id = _usbRequestNo,
				UsbAddress = bytes[4],
				SelfAddress = bytes[5],
				FuncCode = bytes[6]
			};
			UsbRequests.Add(usbRequest);
            return UsbRunner.AddRequest(new List<List<byte>> { bytes }, 1000, 100);
		}

		public static List<JournalItem> GetSecJournalItems2Op(Device device)
		{
			int lastindex = GetLastSecJournalItemId2Op(device);
			var journlaItems = new List<JournalItem>();
			for (int i = 0; i <= lastindex; i++)
			{
				var bytes = new List<byte>();
				bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
				bytes.Add(Convert.ToByte(device.IntAddress / 256));
				bytes.Add(Convert.ToByte(device.IntAddress % 256));
				bytes.Add(0x01);
				bytes.Add(0x20);
				bytes.Add(0x02);
				bytes.AddRange(BitConverter.GetBytes(i).Reverse());
				ParseJournal(SendCode(bytes).Result.FirstOrDefault().Data, journlaItems);
			}
			journlaItems = journlaItems.OrderByDescending(x => x.IntDate).ToList();
			int no = 0;
			foreach (var item in journlaItems)
			{
				no++;
				item.No = no;
			}
			return journlaItems;
		}

		public static int GetLastSecJournalItemId2Op(Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			bytes.Add(Convert.ToByte(device.IntAddress / 256));
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x21);
			bytes.Add(0x02);
			try
			{
				var lastindex = SendCode(bytes);
				int li = 256 * lastindex.Result.FirstOrDefault().Data[9] + lastindex.Result.FirstOrDefault().Data[10];
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
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return GetLastJournalItemId(device) + GetLastSecJournalItemId2Op(device);
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			//bytes.Add(Convert.ToByte(device.IntAddress / 256));
			bytes.Add(0x03); // 1 шлейф
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x24);
			bytes.Add(0x00);
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
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
				return 0;
			return GetLastJournalItemId(device) - GetJournalCount(device) + 1;
		}

		public static int GetLastJournalItemId(Device device)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			//bytes.Add(Convert.ToByte(device.IntAddress / 256));
			bytes.Add(0x03); // 1 шлейф
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x21);
			bytes.Add(0x00);
			try
			{
				var lastindex = SendCode(bytes);
				int li = 256 * lastindex.Result.FirstOrDefault().Data[9] + lastindex.Result.FirstOrDefault().Data[10];
				return li;
			}
			catch (NullReferenceException ex)
			{
				MessageBox.Show(ex.Message);
				throw;
			}
		}

		public static List<JournalItem> GetAllJournalItems(Device device)
		{
			int lastindex = GetLastJournalItemId(device);
			int firstindex = GetFirstJournalItemId(device);
			return GetJournalItems(device, lastindex, firstindex);
		}

		public static List<JournalItem> GetJournalItems(Device device, int lastindex, int firstindex)
		{
			var journalItems = new List<JournalItem>();
			var secJournalItems = new List<JournalItem>();
			if (device.PresentationName == "Прибор РУБЕЖ-2ОП")
			{
				secJournalItems = GetSecJournalItems2Op(device);
			}
			if (firstindex <= lastindex)
				for (int i = firstindex; i <= lastindex; i++)
				{
					ReadItem(device, journalItems, i);
				}
			else
			{
				for (int i = firstindex; i <= 1024; i++)
				{
					ReadItem(device, journalItems, i);
				}
				for (int i = 0; i <= lastindex; i++)
				{
					ReadItem(device, journalItems, i);
				}
			}
			int no = firstindex;
			foreach (var item in journalItems)
			{
				item.No = no;
				no++;
			}
			secJournalItems.ForEach(x => journalItems.Add(x)); // в случае, если устройство не Рубеж-2ОП, коллекция охранных событий будет пустая
			return journalItems;
		}

		private static void ReadItem(Device device, List<JournalItem> journalItems, int i)
		{
			var bytes = new List<byte>();
			bytes.AddRange(BitConverter.GetBytes(++_usbRequestNo).Reverse());
			//bytes.Add(Convert.ToByte(device.IntAddress / 256));
			bytes.Add(0x03); // 1 шлейф
			bytes.Add(Convert.ToByte(device.IntAddress % 256));
			bytes.Add(0x01);
			bytes.Add(0x20);
			bytes.Add(0x00);
			bytes.AddRange(BitConverter.GetBytes(i).Reverse());
			ParseJournal(SendCode(bytes).Result.FirstOrDefault().Data, journalItems);
		}
	}

	internal class UsbRequest
	{
		public int Id { get; set; }

		public int UsbAddress { get; set; }

		public int SelfAddress { get; set; }

		public int FuncCode { get; set; }
	}
}