using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using FiresecAPI.Models;
using ServerFS2;
using ServerFS2.DataBase;
using System.Threading;
using FS2Api;

namespace MonitorClientFS2
{
	public class MonitoringDevice
	{
		const int maxMessages = 1024;
		const int maxSecMessages = 1024;
		public const int betweenDevicesSpan = 0;
		public const int betweenCyclesSpan = 0;
		public const int requestExpiredTime = 10; // in seconds
		public static readonly object Locker = new object();

		static int UsbRequestNo = 1;
		public static event Action<FSJournalItem> OnNewItem;
		public static event Action<List<FSJournalItem>> OnNewItems;

		public MonitoringDevice()
		{
		}

		public MonitoringDevice(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastDisplayedRecord = XmlJournalHelper.GetLastId(device);
			//LastDisplayedRecord = -1;
			FirstDisplayedRecord = -1;
		}		

		public Device Device { get; set; }
		public List<Request> Requests { get; set; }
		public int FirstDisplayedRecord { get; set; }
		public int AnsweredCount { get; set; }
		public int UnAnsweredCount { get; set; }
		public int LastDisplayedRecord
		{
			get { return lastDisplayedRecord; }
			set
			{
				lastDisplayedRecord = value;
				XmlJournalHelper.SetLastId(Device, value);
			}
		}

		public void CheckForExpired()
		{
			foreach (var request in Requests)
			{
				if ((DateTime.Now - request.StartTime).TotalSeconds >= requestExpiredTime)
				{
					Requests.Remove(request);
					UnAnsweredCount++;
				}
			}
		}

		int lastDisplayedRecord;
		
		public void SendRequest(Request request)
		{
			lock (Locker)
			{
				Requests.Add(request);
			}
			JournalHelper.SendByteCommand(request.Bytes, Device, request.Id);
			Thread.Sleep(betweenDevicesSpan);
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
			if (lastDeviceRecord - LastDisplayedRecord > maxMessages)
			{
				LastDisplayedRecord = lastDeviceRecord - maxMessages;
			}
			if (lastDeviceRecord > LastDisplayedRecord)
			{
				Trace.WriteLine(Device.PresentationAddressAndName + " ReadIndex Response " + (lastDeviceRecord - FirstDisplayedRecord));
				GetNewItems(lastDeviceRecord, lastDisplayedRecord);
			}
		}

		bool IsLastIndexValid(Response response)
		{
			return response.Data.Count() >= 10;
		}
		

		void GetNewItems(int lastDeviceRecord, int lastDisplayedRecord)
		{
			Trace.WriteLine("Дочитываю записи с " + LastDisplayedRecord.ToString() + " до " + lastDeviceRecord.ToString());
			MonitoringProcessor.StopMonitoring();
			Thread.Sleep(1000);
			Requests.RemoveAll(x => x.RequestType == RequestTypes.ReadIndex);
			var thread = new Thread(()=>
			{
				List<FSJournalItem> journalItems;
				journalItems = JournalHelper.GetJournalItems(Device, lastDeviceRecord, lastDisplayedRecord+1);
				LastDisplayedRecord = lastDeviceRecord;
				MonitoringProcessor.StartMonitoring();
				foreach (var item in journalItems)
				{
					if (item == null)
					{
						Trace.WriteLine("Запись не считана");
					}
					else
					{
						DBJournalHelper.AddJournalItem(item);
						OnNewItem(item);
					}
				}
				
			});
			thread.Start();
		}

	}

	public class SecMonitoringDevice : MonitoringDevice
	{
		public SecMonitoringDevice(Device device)
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