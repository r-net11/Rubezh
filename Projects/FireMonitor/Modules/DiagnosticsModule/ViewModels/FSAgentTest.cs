using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FSAgentClient;
using Infrastructure;
using System.Windows;
using FiresecClient;
using FiresecAPI.Models;
using Infrastructure.Events;
using Infrastructure.Common;

namespace DiagnosticsModule.ViewModels
{
	public class FSAgentTest
	{
		public FSAgent FSAgent { get; private set; }

		public void Start()
		{
			FSAgent = new FSAgent(AppSettingsManager.FSAgentServerAddress);
			FSAgent.Start();
			SubscribeEvents();
		}

		public void Stop()
		{
			FSAgent = new FSAgent("net.pipe://127.0.0.1/FSAgent/");
			FSAgent.Start();
			SubscribeEvents();
		}

		public void SubscribeEvents()
		{
			FiresecManager.FiresecDriver.Watcher.DevicesStateChanged += new Action<List<DeviceState>>((x) => { SafeCall(() => { OnDeviceStateChangedEvent(x); }); });
			FiresecManager.FiresecDriver.Watcher.DevicesParametersChanged += new Action<List<DeviceState>>((x) => { SafeCall(() => { OnDeviceParametersChangedEvent(x); }); });
			FiresecManager.FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>((x) => { SafeCall(() => { OnZoneStateChangedEvent(x); }); });
			FiresecManager.FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>((x) => { SafeCall(() => { OnNewJournalRecordEvent(x); }); });
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
			FiresecManager.FiresecService.AddJournalRecords(journalRecords);
			ServiceFactory.Events.GetEvent<NewJournalRecordsEvent>().Publish(journalRecords);
		}

		public static void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}
	}
}