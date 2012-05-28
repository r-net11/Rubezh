using System;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Client.Login;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using Infrastructure;

namespace Infrastructure
{
	public class ServiceFactory
	{
		public static void Initialize(ILayoutService ILayoutService, IUserDialogService IUserDialogService, ISecurityService ISecurityService, IWaitService IWaitService)
		{
			Events = new EventAggregator();
			ResourceService = new ResourceService();
			Layout = ILayoutService;
			UserDialogs = IUserDialogService;
			SecurityService = ISecurityService;
			WaitService = IWaitService;
			LoginService = new LoginService(UserDialogs, "Монитор", "Оперативная задача. Авторизация.");
			SubscribeEvents();
		}

		public static AppSettings AppSettings { get; set; }
		public static IEventAggregator Events { get; private set; }
		public static ResourceService ResourceService { get; private set; }
		public static ILayoutService Layout { get; private set; }
		public static IUserDialogService UserDialogs { get; private set; }
		public static ISecurityService SecurityService { get; private set; }
		public static IWaitService WaitService { get; private set; }
		public static LoginService LoginService { get; private set; }
		public static Window ShellView { get; set; }

		static void SubscribeEvents()
		{
			//FiresecEventSubscriber.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournaRecord);
			//FiresecCallbackService.NewJournalRecordEvent += new Action<JournalRecord>(OnNewJournaRecordDispatched);

			FiresecCallbackService.DeviceStateChangedEvent += new Action<Guid>((deviceUID) => { SafeCall(() => { ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Publish(deviceUID); }); });
			FiresecCallbackService.DeviceParametersChangedEvent += new Action<Guid>((deviceUID) => { SafeCall(() => { ServiceFactory.Events.GetEvent<DeviceParametersChangedEvent>().Publish(deviceUID); }); });
			FiresecCallbackService.ZoneStateChangedEvent += new Action<ulong>((zoneNo) => { SafeCall(() => { ServiceFactory.Events.GetEvent<ZoneStateChangedEvent>().Publish(zoneNo); }); });
			FiresecCallbackService.NewJournalRecordEvent += new Action<JournalRecord>((journalRecord) => { SafeCall(() => { ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Publish(journalRecord); }); });
		}

		static void OnNewJournaRecord(JournalRecord journalRecord)
		{
			ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Publish(journalRecord);
		}
		static void OnNewJournaRecordDispatched(JournalRecord journalRecord)
		{
			if (ServiceFactory.ShellView != null)
				ServiceFactory.ShellView.Dispatcher.Invoke(new Action(() =>
				{
					ServiceFactory.Events.GetEvent<NewJournalRecordEvent>().Publish(journalRecord);
				}));
		}

		static void SafeCall(Action action)
		{
			if (ServiceFactory.ShellView != null)
				ServiceFactory.ShellView.Dispatcher.Invoke(new Action(() =>
				{
					action();
				}));
		}
	}
}