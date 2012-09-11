using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Client.Login;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;

namespace Infrastructure
{
	public class ServiceFactory : ServiceFactoryBase
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
		public static ILayoutService Layout { get; private set; }
		public static ISecurityService SecurityService { get; private set; }
		public static LoginService LoginService { get; private set; }

		static void SubscribeEvents()
		{
			FiresecManager.DeviceStateChangedEvent += new Action<DeviceState>((deviceState) => { SafeCall(() => { OnDeviceStateChangedEvent(deviceState); }); });
			FiresecManager.DeviceParametersChangedEvent += new Action<DeviceState>((deviceState) => { SafeCall(() => { OnDeviceParametersChangedEvent(deviceState); }); });
			FiresecManager.ZoneStateChangedEvent += new Action<ZoneState>((zoneState) => { SafeCall(() => { OnZoneStateChangedEvent(zoneState); }); });
			FiresecManager.NewJournalRecordEvent += new Action<JournalRecord>((journalRecord) => { SafeCall(() => { OnNewJournalRecordEvent(journalRecord); }); });
			FiresecCallbackService.NewJournalRecordEvent += new Action<JournalRecord>((journalRecord) => { SafeCall(() => { OnNewJournalRecordEvent(journalRecord); }); });
			FiresecCallbackService.GetFilteredArchiveCompletedEvent += new Action<IEnumerable<JournalRecord>>((journalRecords) => { SafeCall(() => { OnGetFilteredArchiveCompletedEvent(journalRecords); }); });
			FiresecCallbackService.NotifyEvent += new Action<string>((message) => { SafeCall(() => { OnNotify(message); }); });
		}
		static void OnDeviceStateChangedEvent(DeviceState deviceState)
		{
			ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Publish(deviceState.UID);
			if (deviceState != null)
			{
				deviceState.OnStateChanged();
			}
		}
		static void OnDeviceParametersChangedEvent(DeviceState deviceState)
		{
			ServiceFactory.Events.GetEvent<DeviceParametersChangedEvent>().Publish(deviceState.UID);
			if (deviceState != null)
			{
				deviceState.OnParametersChanged();
			}
		}
		static void OnZoneStateChangedEvent(ZoneState zoneState)
		{
			ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Publish(zoneState.No);
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

		static void OnNotify(string message)
		{
			ServiceFactory.Events.GetEvent<NotifyEvent>().Publish(message);
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.Invoke(action);
		}
	}
}