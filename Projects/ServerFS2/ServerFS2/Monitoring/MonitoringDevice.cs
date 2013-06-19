using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Service;
using System.Diagnostics;

namespace ServerFS2.Monitoring
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

		public MonitoringDevice(Device device)
		{
			Panel = device;
			Requests = new List<Request>();
			ResetStateIds = new List<string>();
			DevicesToIgnore = new List<Device>();
			CommandItems = new List<CommandItem>();
			LastSystemIndex = XmlJournalHelper.GetLastId(device);
			//LastSystemIndex = -1;
			FirstSystemIndex = -1;
			SequentUnAnswered = 0;
			IsReadingNeeded = false;
			LostConnectionState = Panel.Driver.States.FirstOrDefault(x => x.Name == "Потеря связи с прибором");
			InitializingState = Panel.Driver.States.FirstOrDefault(x => x.Name == "Устройство инициализируется");
			deviceToGetParams = Panel.Children.FirstOrDefault();
		}

		public List<string> ResetStateIds { get; set; }
		public static event Action<FS2JournalItem> NewJournalItem;
		int SequentUnAnswered;
		int lastSystemIndex;
		DriverState LostConnectionState;
		DriverState InitializingState;
		Device deviceToGetParams;

		public bool CanRequestLastIndex = true;
		public DateTime LastIndexDateTime;

		public List<Device> DevicesToIgnore { get; set; }
		public List<Device> DevicesToResetIgnore { get; set; }
		public List<CommandItem> CommandItems { get; set; }
		public int LastDeviceIndex { get; set; }
		public bool IsReadingNeeded { get; set; }
		public bool IsInitialized { get; private set; }
		public bool IsStateRefreshNeeded { get; set; }
		public bool IsManipulationNeeded { get; set; }
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

		public void CheckTasks()
		{
			if (IsReadingNeeded)
			{
				var journalItems = GetNewItems();
				DeviceStatesManager.UpdateDeviceStateJournal(journalItems);
				DeviceStatesManager.UpdateDeviceState(journalItems);
				DeviceStatesManager.UpdatePanelState(Panel);
			}
			if (ResetStateIds != null && ResetStateIds.Count > 0)
			{
				ServerHelper.ResetOnePanelStates(Panel, ResetStateIds);
				ResetStateIds = new List<string>();
				DeviceStatesManager.UpdatePanelState(Panel);
			}

			if (DevicesToIgnore != null && DevicesToIgnore.Count > 0)
			{
				foreach (var deviceToIgnore in DevicesToIgnore)
				{
					USBManager.SendCodeToPanel(Panel, 0x02, 0x54, 0x0B, 0x01, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
				}
				DevicesToIgnore = new List<Device>();
			}
			if (DevicesToResetIgnore != null && DevicesToResetIgnore.Count > 0)
			{
				foreach (var deviceToIgnore in DevicesToResetIgnore)
				{
					USBManager.SendCodeToPanel(Panel, 0x02, 0x54, 0x0B, 0x00, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
				}
				DevicesToResetIgnore = new List<Device>();
			}
			if (CommandItems != null && CommandItems.Count > 0)
			{
				CommandItems.ForEach(x => x.Send());
				CommandItems = new List<CommandItem>();
			}
			if (IsStateRefreshNeeded)
			{
				RefreshStates();
			}
			DeviceStatesManager.GetDeviceCurrentState(deviceToGetParams);
			deviceToGetParams = GetNextDevice(deviceToGetParams);
			CheckForLostConnection();
			RequestLastIndex();
		}

		public void OnResponceRecieved(Request request, Response response)
		{
			AnsweredCount++;
			if (request.RequestType == RequestTypes.ReadIndex)
			{
				LastIndexReceived(response);
			}
			//else if (request.RequestType == RequestTypes.SecReadIndex)
			//{
			//    SecLastIndexReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
			//}
			//else if (request.RequestType == RequestTypes.SecReadItem)
			//{
			//    SecNewItemReceived((deviceResponceRelation as SecDeviceResponceRelation), response);
			//}
			lock (Locker)
				Requests.RemoveAll(x => x != null && x.Id == request.Id);
		}

		public static void OnNewJournalItem(FS2JournalItem fsJournalItem)
		{
			CallbackManager.Add(new FS2Callbac() { JournalItems = new List<FS2JournalItem>() { fsJournalItem } });
			DatabaseHelper.AddJournalItem(fsJournalItem);
			if (NewJournalItem != null)
				NewJournalItem(fsJournalItem);
		}

		public void CheckForLostConnection()
		{
			var requestsToDelete = new List<Request>();
			lock (Locker)
				foreach (var request in Requests)
				{
					if (request != null && (DateTime.Now - request.StartTime).TotalSeconds >= requestExpiredTime)
					{
						requestsToDelete.Add(request);
						UnAnsweredCount++;
						SequentUnAnswered++;
					}
				}
			requestsToDelete.ForEach(x => Requests.Remove(x));
			if (SequentUnAnswered > maxSequentUnAnswered && !Panel.DeviceState.States.Any(x => x.DriverState == LostConnectionState))
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

		public void ToInitializingState()
		{
			Panel.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = InitializingState, Time = DateTime.Now } };
			Panel.GetRealChildren().ForEach(x =>
			{
				x.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = InitializingState, Time = DateTime.Now } };
				x.DeviceState.OnStateChanged();
			});
			Panel.DeviceState.OnStateChanged();
		}

		public void FromInitializingState()
		{
			Panel.DeviceState.States = new List<DeviceDriverState>();
			Panel.GetRealChildren().ForEach(x =>
			{
				x.DeviceState.States = new List<DeviceDriverState>();
				x.DeviceState.OnStateChanged();
			});
			Panel.DeviceState.OnStateChanged();
			IsStateRefreshNeeded = true;
		}

		void ToLostConnectionState()
		{
			Panel.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = LostConnectionState, Time = DateTime.Now } };
			Panel.GetRealChildren().ForEach(x =>
			{
				x.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = LostConnectionState, Time = DateTime.Now } };
				x.DeviceState.OnStateChanged();
			});
			Panel.DeviceState.OnStateChanged();
			MonitoringDevice.OnNewJournalItem(JournalParser.CustomJournalItem(Panel, LostConnectionState.Name));
		}

		void FromLostConnectionState()
		{
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
			for (int i = 0; i < 100; i++)
			{
				DeviceStatesManager.GetStates(Panel, true);
				Trace.WriteLine("DeviceStatesManager.GetStates " + i);
			}
			DeviceStatesManager.UpdatePanelState(Panel, true);
			IsInitialized = true;
		}

		public void RequestLastIndex()
		{
			var request = new Request(RequestTypes.ReadIndex, new List<byte> { 0x21, 0x00 });
			CanRequestLastIndex = false;
			LastIndexDateTime = DateTime.Now;

			lock (Locker)
			{
				Requests.Add(request);
			}
			request.Id = USBManager.SendCodeToPanelAsync(Panel, 0x01, request.Bytes);
			Trace.WriteLine("request.Id=" + request.Id);
			Thread.Sleep(betweenDevicesSpan);
		}

		public void LastIndexReceived(Response response)
		{
			if (response.HasError)
			{
				return;
			}
			SequentUnAnswered = 0;
			if (Panel.DeviceState.States.Any(x => x.DriverState == LostConnectionState))
			{
				FromLostConnectionState();
			}
			CanRequestLastIndex = true;
			LastDeviceIndex = BytesHelper.ExtractInt(response.Bytes, 7);
			if (FirstSystemIndex == -1)
				FirstSystemIndex = LastDeviceIndex;
			if (LastSystemIndex == -1)
			{
				LastSystemIndex = LastDeviceIndex;
			}
			if (LastDeviceIndex - LastSystemIndex > maxMessages)
			{
				LastSystemIndex = LastDeviceIndex - maxMessages;
			}
			if (LastDeviceIndex > LastSystemIndex)
			{
				IsReadingNeeded = true;
			}
		}

		public List<FS2JournalItem> GetNewItems()
		{
			Requests.RemoveAll(x => x != null && x.RequestType == RequestTypes.ReadIndex);
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

		public void SynchronizeTime()
		{
			var setDateTimeProperty = Panel.Properties.FirstOrDefault(x => x.Name == "SetDateTime");
			if (setDateTimeProperty != null && setDateTimeProperty.Value == "1")
			{
				ServerHelper.SynchronizeTime(Panel);
			}
		}

		Device GetNextDevice(Device device)
		{
			Device nextDevice;
			for (int i = device.AddressOnShleif + 1; i < 256; i++)
			{
				nextDevice = Panel.Children.FirstOrDefault(x => x.AddressOnShleif == i);
				if (nextDevice != null)
					return nextDevice;
			}
			return Panel.Children.FirstOrDefault();
		}

		
	}
}