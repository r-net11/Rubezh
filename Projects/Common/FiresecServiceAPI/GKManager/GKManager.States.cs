﻿using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class GKManager
	{
		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				device.InternalState = new GKDeviceInternalState(device);
				device.State = new GKState(device);
			}
			foreach (var direction in Directions)
			{
				direction.InternalState = new GKDirectionInternalState(direction);
				direction.State = new GKState(direction);
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.InternalState = new GKPumpStationInternalState(pumpStation);
				pumpStation.State = new GKState(pumpStation);
			}
			foreach (var mpt in MPTs)
			{
				mpt.InternalState = new GKMPTInternalState(mpt);
				mpt.State = new GKState(mpt);
			}
			foreach (var delay in Delays)
			{
				delay.InternalState = new GKDelayInternalState(delay);
				delay.State = new GKState(delay);
			}
			foreach (var door in Doors)
			{
				door.InternalState = new GKDoorInternalState(door);
				door.State = new GKState(door);
			}
			foreach (var skdZone in SKDZones)
			{
				skdZone.InternalState = new GKSKDZoneInternalState(skdZone);
				skdZone.State = new GKState(skdZone);
			}
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in Devices)
				if (device.IsRealDevice)
				{
					var stateClass = device.State.StateClass;
					if (stateClass < minStateClass)
						minStateClass = device.State.StateClass;
				}
			foreach (var direction in Directions)
				if (direction.State != null && direction.State.StateClass < minStateClass)
					minStateClass = direction.State.StateClass;
			return minStateClass;
		}
	}
}