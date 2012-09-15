using System;
using System.Collections.Generic;
using System.Threading;
using Firesec;
using FiresecAPI.Models;

namespace FiresecClient
{
	public partial class FiresecManager
	{
		public static FiresecDriver FiresecDriver { get; private set; }
		static int lastJournalNo;

		public static void InitializeFiresecDriver(string FS_Address, int FS_Port, string FS_Login, string FS_Password)
		{
			lastJournalNo = FiresecService.FiresecService.GetJournalLastId().Result;
			FiresecDriver = new FiresecDriver(lastJournalNo, FS_Address, FS_Port, FS_Login, FS_Password);
		}

		public static void Synchronyze()
		{
			FiresecDriver.Synchronyze();
		}

		public static void StatrtWatcher(bool mustMonitorStates)
		{
			FiresecDriver.StatrtWatcher(mustMonitorStates);
			if (mustMonitorStates)
			{
				FiresecDriver.Watcher.DevicesStateChanged += new Action<List<FiresecAPI.Models.DeviceState>>(OnDevicesStateChanged);
				FiresecDriver.Watcher.DevicesParametersChanged += new Action<List<DeviceState>>(OnDevicesParametersChanged);
				FiresecDriver.Watcher.ZonesStateChanged += new Action<List<ZoneState>>(OnZonesStateChanged);
				FiresecDriver.Watcher.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
			}
			FiresecDriver.Watcher.Progress += new Action<int, string, int, int>(OnProgress);
		}

		public static void SynchrinizeJournal()
		{
			var journalRecords = FiresecDriver.Watcher.SynchrinizeJournal(lastJournalNo);
			if (journalRecords.Count > 0)
			{
				FiresecService.AddJournalRecords(journalRecords);
			}
		}

		static void OnDevicesStateChanged(List<DeviceState> deviceStates)
		{
			foreach (var deviceState in deviceStates)
			{
				if (DeviceStateChangedEvent != null)
					DeviceStateChangedEvent(deviceState);
			}
		}

		static void OnDevicesParametersChanged(List<DeviceState> newDeviceStates)
		{
			foreach (var deviceState in newDeviceStates)
			{
				if (DeviceParametersChangedEvent != null)
					DeviceParametersChangedEvent(deviceState);
			}
		}

		static void OnZonesStateChanged(List<ZoneState> zoneStates)
		{
			foreach (var zoneState in zoneStates)
			{
				if (ZoneStateChangedEvent != null)
					ZoneStateChangedEvent(zoneState);
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

		public static event Action<DeviceState> DeviceStateChangedEvent;
		public static event Action<DeviceState> DeviceParametersChangedEvent;
		public static event Action<ZoneState> ZoneStateChangedEvent;
		public static event Action<JournalRecord> NewJournalRecordEvent;
		public static event Action<int, string, int, int> ProgressEvent;

		static void SafeOperationCall(Action action)
		{
			var thread = new Thread(new ThreadStart(action));
			thread.Start();
		}
	}
}