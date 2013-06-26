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
		public const int requestExpiredTime = 5;
		public static readonly object Locker = new object();

		int SequentUnAnswered;
		int RealChildIndex;
		public int AnsweredCount { get; set; }
		public int UnAnsweredCount { get; set; }

		public Device Panel { get; private set; }
		List<Device> RealChildren;
		DeviceStatesManager DeviceStatesManager;
		bool IsConnectionLost;

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
			RealChildren = Panel.GetRealChildren();
			DeviceStatesManager = new DeviceStatesManager();
		}

		public void Initialize()
		{
			DeviceStatesManager.CanNotifyClients = false;
			DeviceStatesManager.UpdatePanelChildrenStates(Panel);
			DeviceStatesManager.UpdatePanelState(Panel);
			DeviceStatesManager.CanNotifyClients = true;
		}

		public void CheckTasks()
		{
			if (IsReadingNeeded)
			{
				var journalItems = GetNewItems();
				DeviceStatesManager.UpdateDeviceStateOnJournal(journalItems);
				DeviceStatesManager.UpdatePanelState(Panel);
				UpdatePanelParameters();
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
			DeviceStatesManager.UpdatePanelInnerParameters(Panel);
			
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

		void UpdatePanelParameters()
		{
			var faultyCount = Panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Неисправность")).Count();
			DeviceStatesManager.ChangeParameter(Panel, faultyCount, "Heиcпpaвныx локальных уcтpoйcтв");
			Trace.WriteLine("faultyCount " + faultyCount);
			
			var externalCount = 0;
			DeviceStatesManager.ChangeParameter(Panel, externalCount, "Внешних устройств");
			Trace.WriteLine("externalCount " + externalCount);
			
			var totalCount = Panel.GetRealChildren().Count;
			DeviceStatesManager.ChangeParameter(Panel, totalCount, "Всего устройств");
			Trace.WriteLine("totalCount " + totalCount);
			
			var bypassCount = Panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Аппаратный обход устройства")).Count();
			DeviceStatesManager.ChangeParameter(Panel, bypassCount, "Обойденных устройств");
			Trace.WriteLine("bypassCount " + bypassCount);

			var lostCount = Panel.GetRealChildren().Where(x => x.DeviceState.States.Any(y => y.DriverState.Name == "Потеря связи")).Count();
			DeviceStatesManager.ChangeParameter(Panel, lostCount, "Потерянных устройств" );
			Trace.WriteLine("lostCount " + lostCount);

			DeviceStatesManager.NotifyStateChanged(Panel);
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
			if (SequentUnAnswered > maxSequentUnAnswered)
			{
				OnConnectionLost();
			}
		}

		public void OnConnectionLost()
		{
			if (!IsConnectionLost)
			{
				IsConnectionLost = true;
				Panel.DeviceState.IsPanelConnectionLost = true;
				DeviceStatesManager.ChangeDeviceStates(Panel);
				OnConnectionChanged();
				OnNewJournalItem(JournalParser.CustomJournalItem(Panel, "Потеря связи с прибором"));
			}
		}

		public void OnConnectionAppeared()
		{
			if (IsConnectionLost)
			{
				IsConnectionLost = false;
				Panel.DeviceState.IsPanelConnectionLost = false;
				DeviceStatesManager.ChangeDeviceStates(Panel);
				OnConnectionChanged();
				OnNewJournalItem(JournalParser.CustomJournalItem(Panel, "Связь с прибором восстановлена"));
			}
		}

		public event Action ConnectionChanged;
		void OnConnectionChanged()
		{
			if (ConnectionChanged != null)
				ConnectionChanged();
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
			OnConnectionAppeared();
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