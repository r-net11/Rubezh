using System;
using System.Linq;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Client.Login;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using System.Collections.Generic;

namespace Infrastructure
{
	public class ServiceFactory
	{
		public static void Initialize(ILayoutService ILayoutService, ISecurityService ISecurityService)
		{
			Events = new EventAggregator();
			ResourceService = new ResourceService();
			Layout = ILayoutService;
			SecurityService = ISecurityService;
			LoginService = new LoginService(ClientType.Monitor, "Оперативная задача. Авторизация.");
			SubscribeEvents();
		}

		public static AppSettings AppSettings { get; set; }
		public static IEventAggregator Events { get; private set; }
		public static ResourceService ResourceService { get; private set; }
		public static ILayoutService Layout { get; private set; }
		public static ISecurityService SecurityService { get; private set; }
		public static LoginService LoginService { get; private set; }

		static void SubscribeEvents()
		{
			FiresecCallbackService.DeviceStateChangedEvent += new Action<Guid>((deviceUID) => { SafeCall(() => { OnDeviceStateChangedEvent(deviceUID); }); });
			FiresecCallbackService.DeviceParametersChangedEvent += new Action<Guid>((deviceUID) => { SafeCall(() => { OnDeviceParametersChangedEvent(deviceUID); }); });
			FiresecCallbackService.ZoneStateChangedEvent += new Action<ulong>((zoneNo) => { SafeCall(() => { OnZoneStateChangedEvent(zoneNo); }); });
			FiresecCallbackService.NewJournalRecordEvent += new Action<JournalRecord>((journalRecord) => { SafeCall(() => { OnNewJournalRecordEvent(journalRecord); }); });
			FiresecCallbackService.GetFilteredArchiveCompletedEvent += new Action<IEnumerable<JournalRecord>>((journalRecords) => { SafeCall(() => { OnGetFilteredArchiveCompletedEvent(journalRecords); }); });
		}

		static void OnDeviceStateChangedEvent(Guid deviceUID)
		{
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Publish(deviceUID);
			var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
			if (deviceState != null)
			{
				deviceState.OnStateChanged();
			}
		}

		static void OnDeviceParametersChangedEvent(Guid deviceUID)
		{
			ServiceFactory.Events.GetEvent<DeviceParametersChangedEvent>().Publish(deviceUID);
			var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == deviceUID);
			if (deviceState != null)
			{
				deviceState.OnParametersChanged();
			}
		}

		static void OnZoneStateChangedEvent(ulong zoneNo)
		{
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Publish(zoneNo);
			var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == zoneNo);
			if (zoneState != null)
			{
				zoneState.OnStateChanged();
			}
		}

		static void OnNewJournalRecordEvent(JournalRecord journalRecord)
		{
			ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Publish(journalRecord);
		}

		static void OnGetFilteredArchiveCompletedEvent(IEnumerable<JournalRecord> journalRecords)
		{
			ServiceFactory.Events.GetEvent<GetFilteredArchiveCompletedEvent>().Publish(journalRecords);
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.Invoke(action);
		}
	}
}