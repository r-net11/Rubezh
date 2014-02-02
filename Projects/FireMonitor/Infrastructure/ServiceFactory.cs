using System;
using System.Collections.Generic;
using System.Windows;
using Common;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Client.Login;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using Infrastructure.Common.Services;
using Infrastructure.Common.Services.Content;
using XFiresecAPI;

namespace Infrastructure
{
	public class ServiceFactory : ServiceFactoryBase
	{
		public static AppSettings AppSettings { get; set; }
		public static ILayoutService Layout { get; private set; }
		public static ISecurityService SecurityService { get; private set; }
		public static LoginService LoginService { get; private set; }
		static bool IsSubcsribed = false;

		public static void Initialize(ILayoutService ILayoutService, ISecurityService ISecurityService)
		{
			ServiceFactoryBase.Events = Events = new EventAggregator();
			ServiceFactoryBase.SecurityService = SecurityService = ISecurityService;
			ResourceService = new ResourceService();
			Layout = ILayoutService;
			LoginService = new LoginService(ClientType.Monitor, "Оперативная задача. Авторизация.");
			ContentService = new ContentService("Monitor");
		}

		public static void SubscribeEvents()
		{
			if (FiresecManager.FiresecDriver == null)
			{
				Logger.Error("ServiceFactory.SubscribeEvents FiresecManager.FiresecDriver = null");
				return;
			}
			if (FiresecManager.FiresecDriver.Watcher == null)
			{
				Logger.Error("ServiceFactory.SubscribeEvents FiresecManager.FiresecDriver.Watcher = null");
				return;
			}

			if (!IsSubcsribed)
			{
				SafeFiresecService.NewJournalRecordEvent += new Action<JournalRecord>((x) => { SafeCall(() => { OnNewServerJournalRecordEvent(new List<JournalRecord>() { x }); }); });
				SafeFiresecService.GetFilteredArchiveCompletedEvent += new Action<IEnumerable<JournalRecord>>((x) => { SafeCall(() => { OnGetFilteredArchiveCompletedEvent(x); }); });

				FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>((x) => { SafeCall(() => { OnDeviceStateChangedEvent(x); }); });
				FiresecManager.FiresecDriver.Watcher.DevicesParametersChanged += new Action<List<DeviceState>>((x) => { SafeCall(() => { OnDeviceParametersChangedEvent(x); }); });
				FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>((x) => { SafeCall(() => { OnZoneStateChangedEvent(x); }); });
				FiresecManager.FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>((x) => { SafeCall(() => { OnNewJournalRecordEvent(x); }); });
			}
			IsSubcsribed = true;
		}

		static void OnDeviceStateChangedEvent(List<DeviceState> deviceStates)
		{
			foreach (var deviceState in deviceStates)
			{
				if (deviceState != null)
				{
					deviceState.OnStateChanged();
				}
			}
			ServiceFactory.Events.GetEvent<DevicesStateChangedEvent>().Publish(null);
		}
		static void OnDeviceParametersChangedEvent(List<DeviceState> deviceStates)
		{
			foreach (var deviceState in deviceStates)
			{
				ServiceFactory.Events.GetEvent<DeviceParametersChangedEvent>().Publish(deviceState.Device.UID);
				if (deviceState != null)
				{
					deviceState.OnParametersChanged();
				}
			}
		}
		static void OnZoneStateChangedEvent(List<ZoneState> zoneStates)
		{
			foreach (var zoneState in zoneStates)
			{
				ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Publish(zoneState.Zone.UID);
				if (zoneState != null)
				{
					zoneState.OnStateChanged();
				}
			}
		}

		static void OnNewJournalRecordEvent(List<JournalRecord> journalRecords)
		{
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Publish(journalRecords);
		}

		static void OnNewServerJournalRecordEvent(List<JournalRecord> journalRecords)
		{
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Publish(journalRecords);
		}

		static void OnGetFilteredArchiveCompletedEvent(IEnumerable<JournalRecord> journalRecords)
		{
			ServiceFactory.Events.GetEvent<GetFilteredArchiveCompletedEvent>().Publish(journalRecords);
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}