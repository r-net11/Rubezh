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
	public class MonitoringPanel
	{
		const int maxSequentUnAnswered = 1000;
		const int maxMessages = 1024;
		const int maxSecMessages = 1024;
		public const int betweenDevicesSpan = 0;
		public const int betweenCyclesSpan = 1000;
		public const int requestExpiredTime = 10; // in seconds
		public static readonly object Locker = new object();

		int SequentUnAnswered;
		DriverState LostConnectionState;
		DriverState InitializingState;
		int RealChildIndex;

		public Device Panel { get; private set; }
		List<Device> RealChildren;
		DeviceStatesManager DeviceStatesManager;

		public static event Action<FS2JournalItem> NewJournalItem;
		public List<string> ResetStateIds { get; set; }
		public List<Device> DevicesToIgnore { get; set; }
		public List<Device> DevicesToResetIgnore { get; set; }
		public List<CommandItem> CommandItems { get; set; }
		public int LastDeviceIndex { get; set; }
		public bool IsReadingNeeded { get; set; }
		public bool IsStateRefreshNeeded { get; set; }
		public bool IsManipulationNeeded { get; set; }
		public List<Request> Requests { get; set; }
		public int FirstSystemIndex { get; set; }
		public int AnsweredCount { get; set; }
		public int UnAnsweredCount { get; set; }

		int _lastSystemIndex;
		public int LastSystemIndex
		{
			get { return _lastSystemIndex; }
			set
			{
				_lastSystemIndex = value;
				XmlJournalHelper.SetLastId(Panel, value);
			}
		}

		public MonitoringPanel(Device device)
		{
			Panel = device;
			Requests = new List<Request>();
			ResetStateIds = new List<string>();
			DevicesToIgnore = new List<Device>();
			CommandItems = new List<CommandItem>();
			LastSystemIndex = XmlJournalHelper.GetLastId(device);
			FirstSystemIndex = -1;
			SequentUnAnswered = 0;
			IsReadingNeeded = false;
			LostConnectionState = Panel.Driver.States.FirstOrDefault(x => x.Name == "Потеря связи с прибором");
			InitializingState = Panel.Driver.States.FirstOrDefault(x => x.Name == "Устройство инициализируется");
			RealChildren = Panel.GetRealChildren();
			DeviceStatesManager = new DeviceStatesManager();
		}

		public void Initialize()
		{
			DeviceStatesManager.UpdatePanelChildrenStates(Panel, true);
			DeviceStatesManager.UpdatePanelState(Panel, true);
		}

		public void CheckTasks()
		{
			if (IsReadingNeeded)
			{
				var journalItems = GetNewItems();
				DeviceStatesManager.UpdateDeviceStateOnJournal(journalItems);
				DeviceStatesManager.UpdatePanelState(Panel);
			}
			if (ResetStateIds != null && ResetStateIds.Count > 0)
			{
				ServerHelper.ResetOnePanelStates(Panel, ResetStateIds);
				ResetStateIds.Clear();
				DeviceStatesManager.UpdatePanelState(Panel);
				OnNewJournalItem(JournalParser.CustomJournalItem(Panel, "Команда оператора. Сброс"));
			}

			if (DevicesToIgnore != null && DevicesToIgnore.Count > 0)
			{
				foreach (var deviceToIgnore in DevicesToIgnore)
				{
					USBManager.Send(Panel, 0x02, 0x54, 0x0B, 0x01, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
				}
				DevicesToIgnore = new List<Device>();
			}
			if (DevicesToResetIgnore != null && DevicesToResetIgnore.Count > 0)
			{
				foreach (var deviceToIgnore in DevicesToResetIgnore)
				{
					USBManager.Send(Panel, 0x02, 0x54, 0x0B, 0x00, 0x00, deviceToIgnore.AddressOnShleif, 0x00, 0x00, 0x00, deviceToIgnore.ShleifNo - 1);
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
				DeviceStatesManager.UpdatePanelState(Panel);
				foreach (var device in RealChildren)
				{
					DeviceStatesManager.UpdateDeviceStateAndParameters(device);
				}
				IsStateRefreshNeeded = false;
			}
			DeviceStatesManager.UpdateDeviceStateAndParameters(RealChildren[RealChildIndex]);
			NextIndextoGetParams();
			CheckConnectionLost();
			RequestLastIndex();
		}

		public void OnResponceRecieved(Request request, Response response)
		{
			AnsweredCount++;
			if (request.RequestType == RequestTypes.ReadIndex)
			{
				LastIndexReceived(response);
			}
			lock (Locker)
				Requests.RemoveAll(x => x != null && x.Id == request.Id);
		}

		void OnNewJournalItem(FS2JournalItem fsJournalItem)
		{
			CallbackManager.NewJournalItems(new List<FS2JournalItem>() { fsJournalItem } );
			DatabaseHelper.AddJournalItem(fsJournalItem);
			if (NewJournalItem != null)
				NewJournalItem(fsJournalItem);
		}

		void CheckConnectionLost()
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
				SetLostConnectionState();
			}
		}

		void SetLostConnectionState()
		{
			Panel.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = LostConnectionState, Time = DateTime.Now } };
			Panel.GetRealChildren().ForEach(x =>
			{
				x.DeviceState.States = new List<DeviceDriverState> { new DeviceDriverState { DriverState = LostConnectionState, Time = DateTime.Now } };
				x.DeviceState.OnStateChanged();
			});
			Panel.DeviceState.OnStateChanged();
			OnNewJournalItem(JournalParser.CustomJournalItem(Panel, LostConnectionState.Name));
		}

		void RemoveLostConnectionState()
		{
			Panel.DeviceState.States = new List<DeviceDriverState>();
			Panel.GetRealChildren().ForEach(x =>
			{
				x.DeviceState.States = new List<DeviceDriverState>();
				x.DeviceState.OnStateChanged();
			});
			Panel.DeviceState.OnStateChanged();
			IsStateRefreshNeeded = true;
			OnNewJournalItem(JournalParser.CustomJournalItem(Panel, "Связь с прибором восстановлена"));
		}

		void NextIndextoGetParams()
		{
			RealChildIndex++;
			if (RealChildIndex + 1 >= RealChildren.Count)
				RealChildIndex = 0;
		}

		public void RequestLastIndex()
		{
			var request = new Request(RequestTypes.ReadIndex, new List<byte> { 0x21, 0x00 });
			lock (Locker)
			{
				Requests.Add(request);
			}
			request.Id = USBManager.SendAsync(Panel, 0x01, request.Bytes);
			if (betweenDevicesSpan > 0)
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
				RemoveLostConnectionState();
			}
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
					OnNewJournalItem(journalItem);
					journalItems.Add(journalItem);
				}
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
	}
}