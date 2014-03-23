using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI;
using System.Threading;
using Common;
using XFiresecAPI;
using FiresecAPI.XModels;

namespace SKDDriver
{
	public partial class Watcher
	{
		void OnDeviceStateChanged(SKDDevice device)
		{
			AddDeviceStateToSKDStates(SKDCallbackResult.SKDStates, device);
			if (device.Zone != null)
			{
				OnZoneStateChanged(device.Zone);
			}
		}

		void OnZoneStateChanged(SKDZone zone)
		{
			var stateClasses = new HashSet<XStateClass>();
			foreach (var device in SKDManager.Devices)
			{
				if (device.ZoneUID == zone.UID)
				{
					foreach (var stateClass in device.State.StateClasses)
					{
						stateClasses.Add(stateClass);
					}
				}
			}
			var hasDifference = stateClasses.Count != zone.State.StateClasses.Count;
			foreach (var stateClass in stateClasses)
			{
				if (!zone.State.StateClasses.Contains(stateClass))
					hasDifference = true;
			}
			if (hasDifference)
			{
				zone.State.StateClasses = stateClasses.ToList();
			}
			zone.State.StateClass = XStatesHelper.GetMinStateClass(zone.State.StateClasses);
			AddZoneStateToSKDStates(SKDCallbackResult.SKDStates, zone);
		}

		public static void AddDeviceStateToSKDStates(SKDStates skdStates, SKDDevice device)
		{
			if (device.State != null)
			{
				device.State.StateClasses = GetStateClasses(device.State);
				device.State.StateClass = XStatesHelper.GetMinStateClass(device.State.StateClasses);
				skdStates.DeviceStates.RemoveAll(x => x.UID == device.UID);
				skdStates.DeviceStates.Add(device.State);
			}
		}

		public static void AddZoneStateToSKDStates(SKDStates skdStates, SKDZone zone)
		{
			if (zone.State != null)
			{
				zone.State.StateClass = XStatesHelper.GetMinStateClass(zone.State.StateClasses);
				skdStates.ZoneStates.RemoveAll(x => x.UID == zone.UID);
				skdStates.ZoneStates.Add(zone.State);
			}
		}

		static List<XStateClass> GetStateClasses(SKDDeviceState deviceState)
		{
			if (deviceState.IsSuspending)
			{
				return new List<XStateClass>() { XStateClass.Unknown };
			}
			if (deviceState.IsConnectionLost)
			{
				return new List<XStateClass>() { XStateClass.ConnectionLost };
			}
			if (deviceState.IsDBMissmatch)
			{
				return new List<XStateClass>() { XStateClass.DBMissmatch };
			}
			if (deviceState.IsInitialState)
			{
				return new List<XStateClass>() { XStateClass.Unknown };
			}
			return deviceState.InternalStateClasses;
		}
	}
}