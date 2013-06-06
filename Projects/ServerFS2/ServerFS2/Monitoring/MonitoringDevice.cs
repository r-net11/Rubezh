using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2;
using ServerFS2.DataBase;


namespace ServerFS2.Monitor
{
	public class MonitoringDevice
	{
		const int maxMessages = 1024;
		const int maxSecMessages = 1024;
		public const int betweenDevicesSpan = 0;
		public const int betweenCyclesSpan = 1000;
		public const int requestExpiredTime = 10; // in seconds
		public static readonly object Locker = new object();

		static int UsbRequestNo = 1;
		public static event Action<FS2JournalItem> NewJournalItem;
		
		public static void OnNewJournalItem(FS2JournalItem fsJournalItem)
		{
			if (NewJournalItem != null)
				NewJournalItem(fsJournalItem);
		}

		public MonitoringDevice()
		{
		}

		public MonitoringDevice(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			//LastSystemIndex = XmlJournalHelper.GetLastId(device);
			LastSystemIndex = -1;
			FirstSystemIndex = -1;
			IsReadingNeeded = false;
		}

		public int LastDeviceIndex { get; set; }
		public bool IsReadingNeeded { get; set; }
		public Device Device { get; set; }
		public List<Request> Requests { get; set; }
		public int FirstSystemIndex { get; set; }
		public int AnsweredCount { get; set; }
		public int UnAnsweredCount { get; set; }
		public int LastSystemIndex
		{
			get { return lastSystemIndex; }
			set
			{
				lastSystemIndex = value;
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

		public void Initialize()
		{
			DeviceStatesManager.GetStates(Device);
			DeviceStatesManager.UpdatePanelState(Device);
		}

		int lastSystemIndex;
		
		public void SendRequest(Request request)
		{
			lock (Locker)
			{
				Requests.Add(request);
			}
			JournalHelper.SendByteCommand(request.Bytes, Device, request.Id);
			Thread.Sleep(betweenDevicesSpan);
		}

		public bool CanRequestLastIndex = true;
		public DateTime LastIndexDateTime;
		public bool CanLastIndexBeRequested()
		{
			if ((DateTime.Now - LastIndexDateTime).TotalMilliseconds > 2000)
			{
				CanRequestLastIndex = true;
			}
			return CanRequestLastIndex;
		}

		public void RequestLastIndex()
		{
			lock (Locker)
			{
				++UsbRequestNo;
			}
			var request = new Request(UsbRequestNo, RequestTypes.ReadIndex, new List<byte> { 0x21, 0x00 });
			CanRequestLastIndex = false;
			LastIndexDateTime = DateTime.Now;
			SendRequest(request);
		}

		public void LastIndexReceived(Response response)
		{
			CanRequestLastIndex = true;
			if (!IsLastIndexValid(response))
			{
				return;
			}
			LastDeviceIndex = 256 * 256 * 256 * response.Data[7] + 256 * 256 * response.Data[8] + 256 * response.Data[9] + response.Data[10];
			if (FirstSystemIndex == -1)
				FirstSystemIndex = LastDeviceIndex;
			if (LastSystemIndex == -1)
			{
				//Trace.WriteLine(Device.PresentationAddressAndName + " LastDeviceIndex " + LastDeviceIndex);
				LastSystemIndex = LastDeviceIndex;
			}
			if (LastDeviceIndex - LastSystemIndex > maxMessages)
			{
				LastSystemIndex = LastDeviceIndex - maxMessages;
			}
			//Trace.WriteLine(Device.PresentationAddressAndName + " ReadIndex Response " + (LastDeviceIndex - FirstSystemIndex));
			if (LastDeviceIndex > LastSystemIndex)
			{
				//foreach (var dataItem in response.Data)
				//{
				//    Trace.Write(dataItem + " ");
				//}
				//Trace.WriteLine("");
				//Trace.WriteLine(Device.PresentationAddressAndName + " ReadIndex Response " + (LastDeviceIndex - FirstSystemIndex));
				IsReadingNeeded = true;
			}
			if (LastDeviceIndex < lastSystemIndex)
			{
				//foreach (var dataItem in response.Data)
				//{
				//    Trace.Write(dataItem + " ");
				//}
				//Trace.WriteLine("");
			}
		}

		bool IsLastIndexValid(Response response)
		{
			if (response.Data[6] == 192)
				throw new Exception("Ошибка считывания индекса");
			return (response.Data.Count() == 11 &&
				response.Data[5] == Device.IntAddress &&
				response.Data[6] == 65);
		}


		public List<FS2JournalItem> GetNewItems()
		{
			//Trace.WriteLine("Дочитываю записи с " + LastSystemIndex.ToString() + " до " + LastDeviceIndex.ToString());
			Requests.RemoveAll(x => x!= null && x.RequestType == RequestTypes.ReadIndex);
			var journalItems = new List<FS2JournalItem>();
			for (int i = LastSystemIndex + 1; i <= LastDeviceIndex; i++)
			{
				var journalItem = JournalHelper.ReadItem(Device, i);
				if (journalItem == null)
				{
					//Trace.WriteLine("Запись не считана");
				}
				else
				{
					MonitoringDevice.OnNewJournalItem(journalItem);
					DBJournalHelper.AddJournalItem(journalItem);
				}
				journalItems.Add(journalItem);
				
			}
			LastSystemIndex = LastDeviceIndex;
			IsReadingNeeded = false;
			return journalItems;
		}

	}

	public class SecMonitoringDevice : MonitoringDevice
	{
		public SecMonitoringDevice(Device device)
		{
			Device = device;
			Requests = new List<Request>();
			LastSystemIndex = XmlJournalHelper.GetLastId(device);
			LastDisplayedSecRecord = XmlJournalHelper.GetLastSecId(device);
			FirstSystemIndex = -1;
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