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
			foreach (var zone in Zones)
			{
				zone.InternalState = new GKZoneInternalState(zone);
				zone.State = new GKState(zone);
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
			foreach (var guardZone in GuardZones)
			{
				guardZone.InternalState = new GKGuardZoneInternalState(guardZone);
				guardZone.State = new GKState(guardZone);
			}
			foreach (var door in Doors)
			{
				door.InternalState = new GKDoorInternalState(door);
				door.State = new GKState(door);
			}
			foreach (var code in DeviceConfiguration.Codes)
			{
				code.InternalState = new GKCodeInternalState(code);
				code.State = new GKState(code);
			}
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in Devices)
				if (device.IsRealDevice)
				{
					var stateClass = device.State.StateClass;
					if (device.DriverType == GKDriverType.AM1_T && stateClass == XStateClass.Fire2)
						stateClass = XStateClass.Info;
					if (stateClass < minStateClass)
						minStateClass = device.State.StateClass;
				}
			foreach (var zone in Zones)
				if (zone.State != null && zone.State.StateClass < minStateClass)
					minStateClass = zone.State.StateClass;
			foreach (var direction in Directions)
				if (direction.State != null && direction.State.StateClass < minStateClass)
					minStateClass = direction.State.StateClass;
			return minStateClass;
		}
	}
}