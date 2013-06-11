using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2;
using ServerFS2.Service;
using FiresecAPI;
using ServerFS2.Journal;
using ServerFS2.ConfigurationWriter;


namespace ServerFS2.Monitor
{
	public class MonitoringDevice
	{
		const int maxSequentUnAnswered = 1000;
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
			CallbackManager.Add(new FS2Callbac() { JournalItems = new List<FS2JournalItem>() { fsJournalItem } });
			DatabaseHelper.AddJournalItem(fsJournalItem);
			if (NewJournalItem != null)
				NewJournalItem(fsJournalItem);
		}

		public MonitoringDevice()
		{
		}

		public MonitoringDevice(Device device)
		{
			Panel = device;
			Requests = new List<Request>();
			ResetStateIds = new List<string>();
			StatesToReset = new List<DriverState>();
			DevicesToIgnore = new List<Device>();
			//LastSystemIndex = XmlJournalHelper.GetLastId(device);
			LastSystemIndex = -1;
			FirstSystemIndex = -1;
			SequentUnAnswered = 0;
			IsReadingNeeded = false;
			LostConnectionState = new DeviceDriverState
			{
				DriverState = Panel.Driver.States.FirstOrDefault(x => x.Name == "Потеря связи с прибором"),
				Time = DateTime.Now
			};
			InitializingState = Panel.Driver.States.FirstOrDefault(x => x.Name == "Устройство инициализируется");
		}

		public List<string> ResetStateIds { get; set; }
		public List<DriverState> StatesToReset { get; set; }
		public List<Device> DevicesToIgnore { get; set; }
		public List<Device> DevicesToResetIgnore { get; set; }


		public int LastDeviceIndex { get; set; }
		public bool IsReadingNeeded { get; set; }
		public bool IsInitialized { get; private set; }
		public bool IsStateRefreshNeeded { get; set; }
		public Device Panel { get; set; }
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
				XmlJournalHelper.SetLastId(Panel, value);
			}
		}

		int SequentUnAnswered;
		public void CheckForLostConnection()
		{
			var requestsToDelete = new List<Request>();
			lock(Locker)
				foreach (var request in Requests)
				{
					if (request !=null && (DateTime.Now - request.StartTime).TotalSeconds >= requestExpiredTime)
					{
						requestsToDelete.Add(request);
						UnAnsweredCount++;
						SequentUnAnswered++;
					}
				}
			requestsToDelete.ForEach(x => Requests.Remove(x));
			if (SequentUnAnswered > maxSequentUnAnswered && !Panel.DeviceState.States.Contains(LostConnectionState))
			{
				ToLostConnectionState();
			}
		}

		public void RefreshStates()
		{
			DeviceStatesManager.UpdatePanelState(Panel);
			DeviceStatesManager.UpdateAllDevicesOnPanelState(Panel);
			IsStateRefreshNeeded = false;
		}

		void ToInitializingState()
		{
			return;
			Panel.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState{ DriverState = InitializingState, Time = DateTime.Now }};
			Panel.GetRealChildren().ForEach(x =>
			{
				x.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState{ DriverState = InitializingState, Time = DateTime.Now }};
				x.DeviceState.OnStateChanged();
			});
			Panel.DeviceState.OnStateChanged();
			//MonitoringDevice.OnNewJournalItem(JournalParser.CustomJournalItem(Panel, LostConnectionState.DriverState.Name));
		}

		void ToLostConnectionState()
		{
			return;
			Panel.DeviceState.States = new List<DeviceDriverState> { LostConnectionState };
			Panel.GetRealChildren().ForEach(x =>
			{
				x.DeviceState.States = new List<DeviceDriverState> { LostConnectionState };
				x.DeviceState.OnStateChanged();
			});
			Panel.DeviceState.OnStateChanged();
			MonitoringDevice.OnNewJournalItem(JournalParser.CustomJournalItem(Panel, LostConnectionState.DriverState.Name));
		}

		void FromLostConnectionState()
		{
			return;
			Panel.DeviceState.States = new List<DeviceDriverState>();
			Panel.GetRealChildren().ForEach(x =>
				{
					x.DeviceState.States = new List<DeviceDriverState>();
					x.DeviceState.OnStateChanged();
				});
			Panel.DeviceState.OnStateChanged();
			IsStateRefreshNeeded = true;
			MonitoringDevice.OnNewJournalItem(JournalParser.CustomJournalItem(Panel, "Связь с прибором восстановлена"));
		}

		public void Initialize()
		{
			ToInitializingState();
			DeviceStatesManager.GetStates(Panel);
			DeviceStatesManager.UpdatePanelState(Panel);
			IsInitialized = true;
		}

		int lastSystemIndex;
		DeviceDriverState LostConnectionState;
		DriverState InitializingState;
		
		public void SendRequest(Request request)
		{
			lock (Locker)
			{
				Requests.Add(request);
			}
			JournalHelper.SendByteCommand(request.Bytes, Panel, request.Id);
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
			if (!IsLastIndexValid(response))
			{
				return;
			}
			SequentUnAnswered = 0;
			if (Panel.DeviceState.States.Contains(LostConnectionState))
			{
				FromLostConnectionState();
			}
			CanRequestLastIndex = true;
			
			LastDeviceIndex = BytesHelper.ExtractInt(response.Data, 7);
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
				IsReadingNeeded = true;
			}
		}

		bool IsLastIndexValid(Response response)
		{
			if (response.Data[6] == 192)
				throw new Exception("Ошибка считывания индекса");
			return (response.Data.Count() == 11 &&
				response.Data[5] == Panel.IntAddress &&
				response.Data[6] == 65);
		}

		public List<FS2JournalItem> GetNewItems()
		{
			//Trace.WriteLine("Дочитываю записи с " + LastSystemIndex.ToString() + " до " + LastDeviceIndex.ToString());
			Requests.RemoveAll(x => x!= null && x.RequestType == RequestTypes.ReadIndex);
			var journalItems = new List<FS2JournalItem>();
			for (int i = LastSystemIndex + 1; i <= LastDeviceIndex; i++)
			{
				var journalItem = JournalHelper.ReadItem(Panel, i);
				if (journalItem != null)
				{
					MonitoringDevice.OnNewJournalItem(journalItem);
				}
				journalItems.Add(journalItem);				
			}
			LastSystemIndex = LastDeviceIndex;
			IsReadingNeeded = false;
			return journalItems;
		}

	}

	//public class SecMonitoringDevice : MonitoringDevice
	//{
	//    public SecMonitoringDevice(Device device)
	//    {
	//        Panel = device;
	//        Requests = new List<Request>();
	//        LastSystemIndex = XmlJournalHelper.GetLastId(device);
	//        LastDisplayedSecRecord = XmlJournalHelper.GetLastSecId(device);
	//        FirstSystemIndex = -1;
	//    }

	//    int lastDisplayedSecRecord;
	//    public int LastDisplayedSecRecord
	//    {
	//        get { return lastDisplayedSecRecord; }
	//        set
	//        {
	//            lastDisplayedSecRecord = value;
	//            XmlJournalHelper.SetLastId(Panel, value);
	//        }
	//    }
	//}
}