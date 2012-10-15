using System;
using System.Collections.Generic;
using System.Windows;
using FiresecAPI.Models;
using FiresecClient;
using Infrastructure.Client.Login;
using Infrastructure.Common;
using Infrastructure.Events;
using Microsoft.Practices.Prism.Events;
using Common;
using System.Threading;
using System.Diagnostics;

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
		}

		public static AppSettings AppSettings { get; set; }
		public static ILayoutService Layout { get; private set; }
		public static ISecurityService SecurityService { get; private set; }
		public static LoginService LoginService { get; private set; }

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
            FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>((x) => { SafeCall(() => { OnDeviceStateChangedEvent(x); }); });
            FiresecManager.FiresecDriver.Watcher.DevicesParametersChanged += new Action<List<DeviceState>>((x) => { SafeCall(() => { OnDeviceParametersChangedEvent(x); }); });
            FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>((x) => { SafeCall(() => { OnZoneStateChangedEvent(x); }); });
            FiresecManager.FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>((x) => { SafeCall(() => { OnNewJournalRecordEvent(x); }); });
			FiresecManager.FiresecDriver.Watcher.Progress +=new Func<int,string,int,int,bool>(Watcher_Progress);

            SafeFiresecService.NewJournalRecordEvent += new Action<JournalRecord>((x) => { SafeCall(() => { OnNewServerJournalRecordEvent(new List<JournalRecord>() { x }); }); });
            SafeFiresecService.GetFilteredArchiveCompletedEvent += new Action<IEnumerable<JournalRecord>>((x) => { SafeCall(() => { OnGetFilteredArchiveCompletedEvent(x); }); });
		}

		static bool Watcher_Progress(int arg1, string arg2, int arg3, int arg4)
		{
			SafeCall(() =>
			{
				Trace.WriteLine(arg1.ToString() + " - " + arg2 + " - " + arg3.ToString() + " - " + arg4.ToString());
			});
			return true;
		}
        static void OnDeviceStateChangedEvent(List<DeviceState> deviceStates)
        {
            foreach (var deviceState in deviceStates)
            {
                ServiceFactory.Events.GetEvent<DeviceStateChangedEvent>().Publish(deviceState.Device.UID);
                if (deviceState != null)
                {
                    deviceState.OnStateChanged();
                }
            }
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
            FiresecManager.FiresecService.AddJournalRecords(journalRecords);
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

		static void OnNotify(string message)
		{
			ServiceFactory.Events.GetEvent<NotifyEvent>().Publish(message);
		}

		public static void SafeCall(Action action)
		{
			var thread = new Thread(new ThreadStart(() =>
				{
					if (Application.Current != null && Application.Current.Dispatcher != null)
						Application.Current.Dispatcher.Invoke(action);
				}));
			thread.Start();
		}
	}
}