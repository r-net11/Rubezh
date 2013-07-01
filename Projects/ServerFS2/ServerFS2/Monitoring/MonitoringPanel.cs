using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Journal;
using ServerFS2.Service;
using System.Diagnostics;
using ServerFS2.Operations;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringPanel
	{
		const int MaxSequentUnAnswered = 10;
		const int MaxMessages = 1024;
		public const int RequestExpiredTime = 5;
		public static readonly object Locker = new object();

		public static event Action<FS2JournalItem> NewJournalItem;

		public Device PanelDevice { get; private set; }
		List<Device> RealChildren;
		DeviceStatesManager DeviceStatesManager;
		bool IsConnectionLost;
		public bool IsInitialized { get; private set; }

		public List<Request> Requests { get; private set; }
		bool IsReadingNeeded = false;
		int LastDeviceIndex { get; set; }

		int _lastSystemIndex;
		public int LastSystemIndex
		{
			get { return _lastSystemIndex; }
			set
			{
				_lastSystemIndex = value;
				XmlJournalHelper.SetLastId(PanelDevice, value);
			}
		}

		public MonitoringPanel(Device device)
		{
			PanelDevice = device;
			Requests = new List<Request>();
			ResetStateIds = new List<string>();
			DevicesToIgnore = new List<Device>();
			CommandItems = new List<CommandItem>();
			LastSystemIndex = XmlJournalHelper.GetLastId(device);
			RealChildren = PanelDevice.GetRealChildren();
			DeviceStatesManager = new DeviceStatesManager();
		}

		public bool Initialize()
		{
			DeviceStatesManager.CanNotifyClients = false;
			IsInitialized = DeviceStatesManager.ReadConfigurationAndUpdateStates(PanelDevice);
			if (!IsInitialized)
			{
				IsConnectionLost = true;
				PanelDevice.DeviceState.IsPanelConnectionLost = true;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				return false;
			}
			else
			{
				IsConnectionLost = false;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
			}
			DeviceStatesManager.UpdatePanelState(PanelDevice);
			GetInformationOperationHelper.GetDeviceInformation(PanelDevice);
			DeviceStatesManager.CanNotifyClients = true;
			return true;
		}

		public void CheckTasks()
		{
			if (IsInitialized)
			{
				if (IsReadingNeeded)
				{
					var journalItems = GetNewItems();
					DeviceStatesManager.UpdateDeviceStateOnJournal(journalItems);
					DeviceStatesManager.UpdatePanelState(PanelDevice);
					DeviceStatesManager.UpdatePanelParameters(PanelDevice);
				}
				DoTasks();
			}

			CheckConnectionLost();
			RequestLastIndex();
		}

		public void RequestLastIndex()
		{
			var request = new Request(RequestTypes.ReadIndex, new List<byte> { 0x01, 0x21, 0x00 });
			lock (Locker)
			{
				Requests.Add(request);
			}
			if (PanelDevice.ParentUSB.UID == PanelDevice.UID)
			{
				var response = USBManager.Send(PanelDevice, request.Bytes);
				if (!response.HasError)
				{
					OnResponceRecieved(request, response);
				}
			}
			else
			{
				USBManager.SendAsync(PanelDevice, request);
			}
		}

		public void OnResponceRecieved(Request request, Response response)
		{
			AnsweredCount++;
			if (request.RequestType == RequestTypes.ReadIndex)
			{
				LastIndexReceived(response);
			}
			lock (Locker)
			{
				Requests.RemoveAll(x => x != null && x.Id == request.Id);
			}
		}

		public void LastIndexReceived(Response response)
		{
			if (response.HasError)
			{
				return;
			}
			SequentUnAnswered = 0;
			OnConnectionAppeared();
			if (response.Id > 0)
			{
				if (response.Bytes.Count < 10)
					return;
				LastDeviceIndex = BytesHelper.ExtractInt(response.Bytes, 7);
			}
			else
			{
				if (response.Bytes.Count < 4)
					return;
				LastDeviceIndex = BytesHelper.ExtractInt(response.Bytes, 0);
			}
			if (LastSystemIndex == -1)
			{
				LastSystemIndex = LastDeviceIndex;
			}
			if (LastDeviceIndex - LastSystemIndex > MaxMessages)
			{
				LastSystemIndex = LastDeviceIndex - MaxMessages;
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
				var journalItem = JournalHelper.ReadItem(PanelDevice, i);
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

		void OnNewJournalItem(FS2JournalItem fsJournalItem)
		{
			CallbackManager.NewJournalItems(new List<FS2JournalItem>() { fsJournalItem });
			DatabaseHelper.AddJournalItem(fsJournalItem);
			if (NewJournalItem != null)
				NewJournalItem(fsJournalItem);
		}
	}
}