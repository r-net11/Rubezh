using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using FiresecAPI.Models;

namespace FSAgentClient
{
	public partial class FSAgent
	{
		Thread RunThread;
		bool IsClosing = false;

		public void Start()
		{
			RunThread = new Thread(OnRun);
			RunThread.Start();
		}

		public void Stop()
		{
			if (RunThread != null)
			{
				IsClosing = true;
				RunThread.Join(TimeSpan.FromSeconds(5));
			}
		}

		void OnRun()
		{
			while (true)
			{
				if (IsClosing)
					return;

				Thread.Sleep(TimeSpan.FromSeconds(1));
				var changeResults = Poll(FSAgentFactory.UID);
				if (changeResults != null)
				{
					foreach (var changeResult in changeResults)
					{
						if (changeResult.DeviceStates != null && changeResult.DeviceStates.Count > 0)
							OnDevicesStateChanged(changeResult.DeviceStates);

						if (changeResult.DeviceParameters != null && changeResult.DeviceParameters.Count > 0)
							OnDevicesParametersChanged(changeResult.DeviceParameters);

						if (changeResult.ZoneStates != null && changeResult.ZoneStates.Count > 0)
							OnZonesStateChanged(changeResult.ZoneStates);

						if (changeResult.JournalRecords != null && changeResult.JournalRecords.Count > 0)
							OnNewJournalRecords(changeResult.JournalRecords);
					}
				}
			}
		}

		public event Action<List<DeviceState>> DevicesStateChanged;
		void OnDevicesStateChanged(List<DeviceState> deviceStates)
		{
			if (DevicesStateChanged != null)
				DevicesStateChanged(deviceStates);
		}

		public event Action<List<DeviceState>> DevicesParametersChanged;
		void OnDevicesParametersChanged(List<DeviceState> deviceStates)
		{
			if (DevicesParametersChanged != null)
				DevicesParametersChanged(deviceStates);
		}

		public event Action<List<ZoneState>> ZonesStateChanged;
		void OnZonesStateChanged(List<ZoneState> zoneStates)
		{
			if (ZonesStateChanged != null)
				ZonesStateChanged(zoneStates);
		}

		public event Action<List<JournalRecord>> NewJournalRecords;
		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
			if (NewJournalRecords != null)
				NewJournalRecords(journalRecords);
		}
	}
}