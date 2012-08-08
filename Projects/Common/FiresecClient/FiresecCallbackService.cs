using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
	public class FiresecCallbackService : IFiresecCallbackService
	{
		public void DeviceStateChanged(List<DeviceState> newDeviceStates)
		{
			SafeOperationCall(() =>
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
						parentState.ParentDevice = FiresecManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == parentState.ParentDeviceId);
						if (parentState.ParentDevice != null)
						{
							parentState.DriverState = parentState.ParentDevice.Driver.States.FirstOrDefault(x => x.Code == parentState.Code);
						}
					}

					if (DeviceStateChangedEvent != null)
						DeviceStateChangedEvent(deviceState.UID);
				}
			});
		}

		public void DeviceParametersChanged(List<DeviceState> newDeviceStates)
		{
			SafeOperationCall(() =>
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
			});
		}

		public void ZonesStateChanged(List<ZoneState> newZoneStates)
		{
			SafeOperationCall(() =>
			{
				if (FiresecManager.DeviceStates == null)
					return;

				foreach (var newZoneState in newZoneStates)
				{
					var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == newZoneState.No);
					zoneState.StateType = newZoneState.StateType;
					zoneState.IsOnGuard = FiresecManager.IsZoneOnGuard(zoneState);

					if (ZoneStateChangedEvent != null)
						ZoneStateChangedEvent(zoneState.No);
				}
			});
		}

		public void NewJournalRecords(List<JournalRecord> journalRecords)
		{
			SafeOperationCall(() =>
			{
				foreach (var journalRecord in journalRecords)
				{
					if (NewJournalRecordEvent != null)
						NewJournalRecordEvent(journalRecord);
				}
			});
		}

		public void ConfigurationChanged()
		{
			SafeOperationCall(() =>
			{
				if (ConfigurationChangedEvent != null)
					ConfigurationChangedEvent();
			});
		}

		public void Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (ProgressEvent != null)
				ProgressEvent(stage, comment, percentComplete, bytesRW);
		}

		public Guid Ping()
		{
			try
			{
				return FiresecManager.ClientCredentials.ClientUID;
			}
			catch(Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecCallbackService.Ping");
				return Guid.Empty;
			}
		}

		public void GetFilteredArchiveCompleted(IEnumerable<JournalRecord> journalRecords)
		{
			SafeOperationCall(() =>
			{
				if (GetFilteredArchiveCompletedEvent != null)
					GetFilteredArchiveCompletedEvent(journalRecords);
			});
		}

		public void Notify(string message)
		{
			SafeOperationCall(() =>
			{
				if (NotifyEvent != null)
					NotifyEvent(message);
			});
		}

		public static event Action<Guid> DeviceStateChangedEvent;
		public static event Action<Guid> DeviceParametersChangedEvent;
		public static event Action<int> ZoneStateChangedEvent;
		public static event Action<JournalRecord> NewJournalRecordEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action<int, string, int, int> ProgressEvent;
		public static event Action<IEnumerable<JournalRecord>> GetFilteredArchiveCompletedEvent;
		public static event Action<string> NotifyEvent;

		void SafeOperationCall(Action action)
		{
			try
			{
				var thread = new Thread(new ThreadStart(action));
				thread.Start();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecCallbackService.SafeOperationCall");
			}
		}
	}
}