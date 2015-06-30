﻿using System;
using System.Collections.Generic;
using FiresecAPI.Models;
using FS2Api;
using ServerFS2.Helpers;
using ServerFS2.Journal;
using ServerFS2.Operations;
using ServerFS2.Service;
using System.Text;
using System.Linq;
using System.Diagnostics;

namespace ServerFS2.Monitoring
{
	public partial class MonitoringPanel
	{
		const int MaxFireMessages = 1024;
		const int MaxSecurityMessages = 500;
		public const int RequestExpiredTime = 5;
		public static readonly object Locker = new object();

		public Device PanelDevice { get; private set; }
		List<Device> RealChildren;
		DeviceStatesManager DeviceStatesManager;
		public bool IsInitialized { get; private set; }
		public DeviceConfiguration RemoteDeviceConfiguration { get; private set; }

		public List<Request> Requests { get; private set; }
		bool IsFireReadingNeeded = false;
		bool IsSecurityReadingNeeded = false;
		int LastDeviceFireIndex { get; set; }
		int LastDeviceSecurityIndex { get; set; }
		LastJournalIndexManager LastJournalIndexManager = new LastJournalIndexManager();

		int _lastSystemFireIndex;
		public int LastSystemFireIndex
		{
			get { return _lastSystemFireIndex; }
			set
			{
				_lastSystemFireIndex = value;
				LastJournalIndexManager.SetLastFireJournalIndex(PanelDevice, SerialNo, value);
			}
		}

		int _lastSystemSecurityIndex;
		public int LastSystemSecurityIndex
		{
			get { return _lastSystemSecurityIndex; }
			set
			{
				_lastSystemSecurityIndex = value;
				LastJournalIndexManager.SetLastSecurityJournalIndex(PanelDevice, SerialNo, value);
			}
		}

		public MonitoringPanel(Device device)
		{
			PanelDevice = device;
			Requests = new List<Request>();
			ResetStateIds = new List<string>();
			DevicesToIgnore = new List<Device>();
			ZonesToSetGuard = new List<Zone>();
			ZonesToResetGuard = new List<Zone>();
			CommandItems = new List<CommandItem>();
			RealChildren = PanelDevice.GetRealChildren();
			DeviceStatesManager = new DeviceStatesManager();
		}

		public bool Initialize()
		{
			DeviceStatesManager.CanNotifyClients = false;
			IsInitialized = DeviceStatesManager.ReadConfigurationAndUpdateStates(PanelDevice);
			RemoteDeviceConfiguration = DeviceStatesManager.RemoteDeviceConfiguration;
			if (!IsInitialized)
			{
				PanelDevice.DeviceState.IsPanelConnectionLost = true;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
				return false;
			}
			else
			{
				PanelDevice.DeviceState.IsPanelConnectionLost = false;
				DeviceStatesManager.ForseUpdateDeviceStates(PanelDevice);
			}
			DeviceStatesManager.UpdatePanelState(PanelDevice);
			GetInformationOperationHelper.GetDeviceInformation(PanelDevice);
			DeviceStatesManager.CanNotifyClients = true;
			SerialNo = GetSerialNo();

			LastSystemFireIndex = LastJournalIndexManager.GetLastFireJournalIndex(PanelDevice, SerialNo);
			LastSystemSecurityIndex = LastJournalIndexManager.GetLastSecurityJournalIndex(PanelDevice, SerialNo);

			SynchronyzeJournal(0x00);
			if (PanelDevice.Driver.DriverType == DriverType.Rubezh_2OP || PanelDevice.Driver.DriverType == DriverType.USB_Rubezh_2OP)
			{
				SynchronyzeJournal(0x02);
			}
			return true;
		}

		public void ProcessMonitoring()
		{
			if (IsInitialized)
			{
				if (IsFireReadingNeeded || IsSecurityReadingNeeded)
				{
					var journalItems = new List<FS2JournalItem>();
                    if (IsFireReadingNeeded)
                        journalItems.AddRange(GetNewFireJournalItems());
                    if (IsSecurityReadingNeeded)
                        journalItems.AddRange(GetNewSecurityJournalItems());

					if (journalItems.Any(x => x.Description == "Реконфигурация базы"))
					{
						Initialize();
					}

					if (!PanelDevice.DeviceState.IsWrongPanel && !PanelDevice.DeviceState.IsDBMissmatch)
					{
						DeviceStatesManager.UpdateDeviceStateOnJournal(journalItems);
						DeviceStatesManager.UpdatePanelState(PanelDevice);
						DeviceStatesManager.UpdatePanelParameters(PanelDevice);
					}
				}
				if (!PanelDevice.DeviceState.IsWrongPanel && !PanelDevice.DeviceState.IsDBMissmatch)
				{
					DoTasks();
				}
			}

			CheckConnectionLost();
			RequestLastIndex(0x00);
			if (PanelDevice.Driver.DriverType == DriverType.Rubezh_2OP || PanelDevice.Driver.DriverType == DriverType.USB_Rubezh_2OP)
			{
				RequestLastIndex(0x02);
			}
		}

		public void RequestLastIndex(byte journalType)
		{
			var requestType = RequestType.ReadFireIndex;
			if (journalType == 0x02)
				requestType = RequestType.ReadSecurityIndex;
			var request = new Request(requestType, new List<byte> { 0x01, 0x21, journalType });
			lock (Locker)
			{
				Requests.Add(request);
			}
			if (PanelDevice.ParentUSB.UID == PanelDevice.UID)
			{
				var response = USBManager.SendShortAttempt(PanelDevice, "Запрос индекса последней записи", request.Bytes);
				if (!response.HasError)
				{
					OnResponceRecieved(request, response);
				}
			}
			else
			{
				USBManager.SendAsync(PanelDevice, "Запрос индекса последней записи", request);
			}
		}

		public void OnResponceRecieved(Request request, Response response)
		{
			AnsweredCount++;
			if (request.RequestType == RequestType.ReadFireIndex || request.RequestType == RequestType.ReadSecurityIndex)
			{
				LastIndexReceived(response, request.RequestType);
			}
			lock (Locker)
			{
				Requests.RemoveAll(x => x != null && x.Id == request.Id);
			}
		}

		public void LastIndexReceived(Response response, RequestType requestType)
		{
			if (response.HasError)
			{
				return;
			}
			SequentUnAnswered = 0;
			OnConnectionAppeared();
			var lastDeviceIndex = -1;
			if (response.Id > 0)
			{
				if (response.Bytes.Count < 10)
					return;
				lastDeviceIndex = BytesHelper.ExtractInt(response.Bytes, 7);
			}
			else
			{
				if (response.Bytes.Count != 4)
					return;
				lastDeviceIndex = BytesHelper.ExtractInt(response.Bytes, 0);
			}

			switch (requestType)
			{
				case RequestType.ReadFireIndex:
					LastDeviceFireIndex = lastDeviceIndex;
					if (LastSystemFireIndex == -1)
					{
						LastSystemFireIndex = LastDeviceFireIndex;
						break;
					}
					if (LastDeviceFireIndex - LastSystemFireIndex > MaxFireMessages)
					{
						if (CheckWrongPanel())
						{
							LastSystemFireIndex = LastDeviceFireIndex - MaxFireMessages;
							IsFireReadingNeeded = true;
						}
						break;
					}
					if (LastDeviceFireIndex > LastSystemFireIndex)
					{
						IsFireReadingNeeded = true;
						break;
					}
					if (LastDeviceFireIndex < LastSystemFireIndex)
					{
						if (CheckWrongPanel())
						{
							LastDeviceFireIndex = LastSystemFireIndex;
						}
					}
					break;

				case RequestType.ReadSecurityIndex:
					LastDeviceSecurityIndex = lastDeviceIndex;
					if (LastSystemSecurityIndex == -1)
					{
						LastSystemSecurityIndex = LastDeviceSecurityIndex;
						break;
					}
					if (LastDeviceSecurityIndex - LastSystemSecurityIndex > MaxSecurityMessages)
					{
						if (CheckWrongPanel())
						{
							LastSystemSecurityIndex = LastDeviceSecurityIndex - MaxSecurityMessages;
							IsSecurityReadingNeeded = true;
						}
						break;
					}
					if (LastDeviceSecurityIndex > LastSystemSecurityIndex)
					{
						IsSecurityReadingNeeded = true;
						break;
					}
					if (LastDeviceSecurityIndex < LastSystemSecurityIndex)
					{
						if (CheckWrongPanel())
						{
							LastDeviceSecurityIndex = LastSystemSecurityIndex;
						}
					}
					break;
			}

			var deltaIndex = LastDeviceFireIndex - LastSystemFireIndex;
			if (deltaIndex > 100)
			{
				CallbackManager.AddLog("GetNewJournalItems " + deltaIndex);
			}
		}

		public List<FS2JournalItem> GetNewFireJournalItems(bool doProgress = false)
		{
			Requests.RemoveAll(x => x != null && x.RequestType == RequestType.ReadFireIndex);
			var journalItems = new List<FS2JournalItem>();

			if (LastSystemFireIndex == 0)
				LastSystemFireIndex = LastDeviceFireIndex;

			for (int i = Math.Max(LastDeviceFireIndex - MaxFireMessages, LastSystemFireIndex + 1); i <= LastDeviceFireIndex; i++)
			{
				if (doProgress)
				{
					CallbackManager.AddProgress(new FS2ProgressInfo("Чтение записей журнала " + (i - LastSystemFireIndex).ToString() + " из " + (LastDeviceFireIndex - LastSystemFireIndex).ToString(),
						(i - LastSystemFireIndex) * 100 / (LastDeviceFireIndex - LastSystemFireIndex)));
				}
				var journalItem = JournalHelper.ReadItem(RemoteDeviceConfiguration, PanelDevice, i, 0x00);
				if (journalItem != null)
				{
					OnNewJournalItem(journalItem);
					journalItems.Add(journalItem);
				}
			}

            LastSystemFireIndex = LastDeviceFireIndex;
			IsFireReadingNeeded = false;
			return journalItems;
		}

        public List<FS2JournalItem> GetNewSecurityJournalItems(bool doProgress = false)
        {
            Requests.RemoveAll(x => x != null && x.RequestType == RequestType.ReadSecurityIndex);
            var journalItems = new List<FS2JournalItem>();

			if (LastSystemSecurityIndex == 0)
				LastSystemSecurityIndex = LastDeviceSecurityIndex;

            for (int i = Math.Max(LastDeviceSecurityIndex - MaxSecurityMessages, LastSystemSecurityIndex + 1); i <= LastDeviceSecurityIndex; i++)
            {
                if (doProgress)
                {
                    CallbackManager.AddProgress(new FS2ProgressInfo("Чтение охранных записей журнала " + (i - LastSystemSecurityIndex).ToString() + " из " + (LastDeviceSecurityIndex - LastSystemSecurityIndex).ToString(),
                            (i - LastSystemSecurityIndex) * 100 / (LastDeviceSecurityIndex - LastSystemSecurityIndex)));
                }
                var journalItem = JournalHelper.ReadItem(RemoteDeviceConfiguration, PanelDevice, i, 0x02);
                if (journalItem != null)
                {
                    OnNewJournalItem(journalItem);
                    journalItems.Add(journalItem);
                }
            }
            LastSystemSecurityIndex = LastDeviceSecurityIndex;
            IsSecurityReadingNeeded = false;
            return journalItems;
        }

		void SynchronyzeJournal(int journalType)
		{
			CallbackManager.AddProgress(new FS2ProgressInfo("Чтение индекса последней записи", 50));
			var response = USBManager.Send(PanelDevice, "Запрос индекса последней записи", 0x01, 0x21, journalType);
			if (response.HasError)
				return;
            if (journalType == 0x00)
            {
                LastDeviceFireIndex = BytesHelper.ExtractInt(response.Bytes, 0);
                GetNewFireJournalItems(true);
            }
            if (journalType == 0x02)
            {
                LastDeviceSecurityIndex = BytesHelper.ExtractInt(response.Bytes, 0);
                GetNewSecurityJournalItems(true);
            }
        }

		void OnNewJournalItem(FS2JournalItem fsJournalItem)
		{
			CallbackManager.NewJournalItems(new List<FS2JournalItem>() { fsJournalItem });
			ServerFS2Database.AddJournalItem(fsJournalItem);
		}
	}
}