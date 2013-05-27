using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using ServerFS2;
using ServerFS2.DataBase;
using System.Threading;

namespace MonitorClientFS2
{
	public class MonitoringDevice
	{
		const int maxMessages = 1024;
		const int maxSecMessages = 1024;
		//const int newItemRequestTimeout = 100;
		public const int betweenDevicesSpan = 0;
		public const int betweenCyclesSpan = 0;

		static int UsbRequestNo = 1;
		public static readonly object Locker = new object();

		public Device Device { get; set; }
		public List<Request> Requests { get; set; }
		public int FirstDisplayedRecord { get; set; }
		int lastDisplayedRecord;
		
		public int AnsweredCount { get; set; }
		public int UnAnsweredCount
		{
			get { return Requests.Count; }
		}

		public static event Action<FSJournalItem> OnNewItems;

		public MonitoringDevice()
		{
		}

		public MonitoringDevice(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
			FirstDisplayedRecord = -1;
		}

		public int LastDisplayedRecord
		{
			get { return lastDisplayedRecord; }
			set
			{
				lastDisplayedRecord = value;
				XmlJournalHelper.SetLastId(Device, value);
			}
		}

		public void SendRequest(Request request)
		{
			lock (Locker)
			{
				Requests.Add(request);
			}
			JournalHelper.SendByteCommand(request.Bytes, Device, request.Id);
			Thread.Sleep(betweenDevicesSpan);
		}

		void NewItemRequest(int ItemIndex)
		{
			lock (Locker)
			{
				++UsbRequestNo;
			}
			var bytes = new List<byte> { 0x20, 0x00 };
			bytes.AddRange(BitConverter.GetBytes(ItemIndex).Reverse());
			Request request = new Request(UsbRequestNo, RequestTypes.ReadItem, bytes);
			SendRequest(request);
		}

		public void NewItemRequests(int lastDeviceRecord, int lastDisplayedRecord)
		{
			LastDisplayedRecord = lastDeviceRecord;
			for (int i = lastDisplayedRecord + 1; i <= lastDeviceRecord; i++)
			//for (int i = lastDeviceRecord - 10; i <= lastDeviceRecord; i++)
			{
				NewItemRequest(i);
			}
		}

		public void NewItemReceived(Response response)
		{
			if (!NewItemValid(response))
				return;
			var journalItem = JournalParser.FSParce(response.Data);
			Trace.WriteLine("ReadItem Responce " + Device.PresentationAddressAndName + " " + response.Id);
			DBJournalHelper.AddJournalItem(journalItem);
			OnNewItems(journalItem);
		}

		bool NewItemValid(Response response)
		{
			return response.Data.Count >= 24;
		}

		public void RequestLastIndex()
		{
			lock (Locker)
			{
				++UsbRequestNo;
			}
			var request = new Request(UsbRequestNo, RequestTypes.ReadIndex, new List<byte> { 0x21, 0x00 });
			SendRequest(request);
		}

		public void LastIndexReceived(Response response)
		{
			if (!IsLastIndexValid(response))
				return;
			var lastDeviceRecord = 256 * 256 * 256 * response.Data[7] + 256 * 256 * response.Data[8] + 256 * response.Data[9] + response.Data[10];
			if (FirstDisplayedRecord == -1)
				FirstDisplayedRecord = lastDeviceRecord;
			if (LastDisplayedRecord == -1)
				LastDisplayedRecord = lastDeviceRecord;
			Trace.WriteLine(Device.PresentationAddressAndName + " ReadIndex Response " + (lastDeviceRecord - FirstDisplayedRecord));
			if (lastDeviceRecord - LastDisplayedRecord > maxMessages)
			{
				LastDisplayedRecord = lastDeviceRecord - maxMessages;
			}
			if (lastDeviceRecord > LastDisplayedRecord)
			{
				Trace.WriteLine("Дочитываю записи с " + (LastDisplayedRecord + 1).ToString() + " до " + lastDeviceRecord.ToString());
				//var thread = new Thread(() =>
				//{
				    NewItemRequests(lastDeviceRecord, LastDisplayedRecord);
				//});
				//thread.Start();
			}
		}

		bool IsLastIndexValid(Response response)
		{
			return response.Data.Count() >= 10;
		}
	}

	public class SecDeviceResponceRelation : MonitoringDevice
	{
		public SecDeviceResponceRelation(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
			LastDisplayedSecRecord = XmlJournalHelper.GetLastSecId(device);
			FirstDisplayedRecord = -1;
		}

		int lastDisplayedSecRecord;
		public int LastDisplayedSecRecord
		{
			get { return lastDisplayedSecRecord; }
			set
			{
				lastDisplayedSecRecord = value;
				XmlJournalHelper.SetLastId(Device, value);
			}
		}
	}
}