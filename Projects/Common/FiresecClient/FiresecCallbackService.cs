using System;
using System.Collections.Generic;
using System.Linq;
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

					//deviceState.OnStateChanged();
					if (DeviceStateChangedEvent != null)
						DeviceStateChangedEvent(deviceState.UID);
				}
			});
		}

		public void DeviceParametersChanged(List<DeviceState> newDeviceStates)
		{
			SafeOperationCall(() =>
			{
				foreach (var newDeviceState in newDeviceStates)
				{
					var deviceState = FiresecManager.DeviceStates.DeviceStates.FirstOrDefault(x => x.UID == newDeviceState.UID);
					if (deviceState != null)
					{
						deviceState.Parameters = newDeviceState.Parameters;

						//deviceState.OnParametersChanged();
						if (DeviceParametersChangedEvent != null)
							DeviceParametersChangedEvent(deviceState.UID);
					}
				}
			});
		}

		public void ZoneStateChanged(ZoneState newZoneState)
		{
			SafeOperationCall(() =>
			{
				var zoneState = FiresecManager.DeviceStates.ZoneStates.FirstOrDefault(x => x.No == newZoneState.No);
				zoneState.StateType = newZoneState.StateType;
				zoneState.RevertColorsForGuardZone = IsZoneOnGuard(newZoneState);

				//zoneState.OnStateChanged();
				if (ZoneStateChangedEvent != null)
					ZoneStateChangedEvent(zoneState.No);
			});
		}

		public bool IsZoneOnGuard(ZoneState zoneState)
		{
			var zone = FiresecManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.No == zoneState.No);
			if (zone.ZoneType == ZoneType.Guard)
			{
				foreach (var deviceState in FiresecManager.DeviceStates.DeviceStates)
				{
					if (deviceState.Device.ZoneNo.HasValue)
					{
						if (deviceState.Device.ZoneNo.Value == zone.No)
						{
							if (deviceState.States.Any(x => x.Code == "OnGuard") == false)
								return true;
						}
					}
				}
			}
			return false;
		}

		public void NewJournalRecord(JournalRecord journalRecord)
		{
#if (DEBUG)
			//throw new Exception("Test");
#endif
			SafeOperationCall(() =>
			{
				if (NewJournalRecordEvent != null)
					NewJournalRecordEvent(journalRecord);
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

		public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (ProgressEvent != null)
				return ProgressEvent(stage, comment, percentComplete, bytesRW);
			return true;
		}

		public Guid Ping()
		{
#if (DEBUG)
			//throw new Exception("Test");
#endif
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

		public static event Action<Guid> DeviceStateChangedEvent;
		public static event Action<Guid> DeviceParametersChangedEvent;
		public static event Action<ulong> ZoneStateChangedEvent;
		public static event Action<JournalRecord> NewJournalRecordEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Func<int, string, int, int, bool> ProgressEvent;

		void SafeOperationCall(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове FiresecCallbackService.SafeOperationCall");
			}
		}
	}
}