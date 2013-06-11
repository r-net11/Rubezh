using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecClient;
using FiresecAPI.Models;
using FS2Api;
using Infrastructure;
using Infrastructure.Events;
using System.Windows.Threading;

namespace DevicesModule
{
	public static class FS2Helper
	{
		public static void Initialize()
		{
			FiresecManager.FS2ClientContract.DeviceStateChanged += new Action<List<DeviceState>>(OnDeviceStateChanged);
			FiresecManager.FS2ClientContract.DeviceParametersChanged += new Action<List<DeviceState>>(OnDeviceParametersChanged);
			FiresecManager.FS2ClientContract.ZoneStatesChanged += new Action<List<ZoneState>>(OnZoneStatesChanged);
			FiresecManager.FS2ClientContract.NewJournalItems += new Action<List<FS2JournalItem>>(OnNewJournalItems);
			FiresecManager.FS2ClientContract.NewArchiveJournalItems += new Action<List<FS2JournalItem>>(OnNewArchiveJournalItems);
			FiresecManager.FS2ClientContract.Progress += new Action<FS2ProgressInfo>(OnProgress);
		}

		static void OnDeviceStateChanged(List<DeviceState> deviceStates)
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
				foreach (var deviceState in deviceStates)
				{
					var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceState.DeviceUID);
					if (device != null)
					{
						device.DeviceState.States = deviceState.SerializableStates;
						device.DeviceState.SerializableParentStates = deviceState.SerializableParentStates;
						device.DeviceState.SerializableChildStates = deviceState.SerializableChildStates;
						device.DeviceState.SerializableParameters = deviceState.SerializableParameters;
						device.DeviceState.OnStateChanged();
					}
				}
			}));
		}

		static void OnDeviceParametersChanged(List<DeviceState> deviceStates)
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
				foreach (var deviceState in deviceStates)
				{
					var device = FiresecManager.Devices.FirstOrDefault(x => x.UID == deviceState.DeviceUID);
					if (device != null)
					{
						device.DeviceState.States = deviceState.SerializableStates;
						device.DeviceState.SerializableParentStates = deviceState.SerializableParentStates;
						device.DeviceState.SerializableChildStates = deviceState.SerializableChildStates;
						device.DeviceState.SerializableParameters = deviceState.SerializableParameters;
						device.DeviceState.OnStateChanged();
					}
				}
			}));
		}

		static void OnZoneStatesChanged(List<ZoneState> zoneStates)
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
				foreach (var zoneState in zoneStates)
				{
					var zone = FiresecManager.Zones.FirstOrDefault(x => x.UID == zoneState.ZoneUID);
					if (zone != null)
					{
						zone.ZoneState.StateType = zoneState.StateType;
						zone.ZoneState.OnStateChanged();
					}
				}
			}));
		}

		static void OnNewJournalItems(List<FS2JournalItem> journalItems)
		{
			Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
				{
					ServiceFactory.Events.GetEvent<NewFS2JournalItemsEvent>().Publish(journalItems);
				}));
		}

		static void OnNewArchiveJournalItems(List<FS2JournalItem> journalItems)
		{
			ServiceFactory.Events.GetEvent<GetFS2FilteredArchiveCompletedEvent>().Publish(journalItems);
		}

		static void OnProgress(FS2ProgressInfo progressInfo)
		{
		}
	}
}