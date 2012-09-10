using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI.Models;
using System.Threading;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static FiresecDriver FiresecDriver { get; private set; }

		public static void InitializeFiresecDriver()
		{
			ConfigurationCash.DriversConfiguration = new DriversConfiguration();
			ConfigurationCash.DriversConfiguration.Drivers = FiresecConfiguration.Drivers;
			ConfigurationCash.DeviceConfiguration = FiresecConfiguration.DeviceConfiguration;
			ConfigurationCash.PlansConfiguration = PlansConfiguration;
			ConfigurationCash.DeviceConfigurationStates = new DeviceConfigurationStates();

			var lastJournalNo = FiresecService.FiresecService.GetJournalLastId().Result;
			FiresecDriver = new FiresecDriver(lastJournalNo);
			FiresecDriver.Watcher.DevicesStateChanged += new Action<List<FiresecAPI.Models.DeviceState>>(OnDevicesStateChanged);
			FiresecDriver.Watcher.DevicesParametersChanged += new Action<List<DeviceState>>(OnDevicesParametersChanged);
			FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>(OnZonesStateChanged);
			FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
			FiresecDriver.Watcher.Progress += new Action<int, string, int, int>(OnProgress);

			var journalRecords = FiresecDriver.Watcher.SynchrinizeJournal(lastJournalNo);
			if (journalRecords.Count > 0)
			{
				OnNewJournalRecords(journalRecords);
			}
		}

		static void OnDevicesStateChanged(List<DeviceState> newDeviceStates)
		{
			if (FiresecManager.DeviceStates == null)
				return;

			foreach (var newDeviceState in newDeviceStates)
			{
				var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);
				if (deviceState == null)
					continue;

				deviceState.States.Clear();
				foreach (var newState in newDeviceState.States)
				{
					deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == newState.Code);
					newState.DriverState = deviceState.Device.Driver.States.FirstOrDefault(x => x.Code == newState.Code);
					deviceState.States.Add(newState);
				}

				deviceState.ParentStates = newDeviceState.ParentStates;
				foreach (var parentState in deviceState.ParentStates)
				{
					parentState.ParentDevice = FiresecManager.FiresecConfiguration.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
					if (parentState.ParentDevice != null)
					{
						parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
					}
				}

				if (DeviceStateChangedEvent != null)
					DeviceStateChangedEvent(deviceState.UID);
			}
		}

		static void OnDevicesParametersChanged(List<DeviceState> newDeviceStates)
		{
			if (FiresecManager.DeviceStates == null)
				return;

			foreach (var newDeviceState in newDeviceStates)
			{
				var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);
				if (deviceState != null)
				{
					deviceState.Parameters = newDeviceState.Parameters;

					if (DeviceParametersChangedEvent != null)
						DeviceParametersChangedEvent(deviceState.UID);
				}
			}
		}

		static void OnZonesStateChanged(List<ZoneState> newZoneStates)
		{
			if (FiresecManager.DeviceStates == null)
				return;

			foreach (var newZoneState in newZoneStates)
			{
				var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == newZoneState.No);
				zoneState.StateType = newZoneState.StateType;

				if (ZoneStateChangedEvent != null)
					ZoneStateChangedEvent(zoneState.No);
			}
		}

		static void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			foreach (var journalRecord in journalRecords)
			{
				if (NewJournalRecordEvent != null)
					NewJournalRecordEvent(journalRecord);
			}

			FiresecService.AddJournalRecords(journalRecords);
		}

		static void OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			SafeOperationCall(() =>
			{
				if (ProgressEvent != null)
					ProgressEvent(stage, comment, percentComplete, bytesRW);
			});
		}

		public static event Action<Guid> DeviceStateChangedEvent;
		public static event Action<Guid> DeviceParametersChangedEvent;
		public static event Action<int> ZoneStateChangedEvent;
		public static event Action<JournalRecord> NewJournalRecordEvent;
		public static event Action<int, string, int, int> ProgressEvent;

		static void SafeOperationCall(Action action)
		{
			var thread = new Thread(new ThreadStart(action));
			thread.Start();
		}
	}
}