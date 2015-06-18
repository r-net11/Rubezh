using FiresecAPI.GK;

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
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.InternalState = new GKPumpStationInternalState(pumpStation);
				pumpStation.State = new GKState(pumpStation);
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
			return minStateClass;
		}
	}
}