using RubezhAPI.GK;

namespace RubezhAPI
{
	public partial class GKManager
	{
		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				device.InitializeInternalState();
			    if (device.DriverType == GKDriverType.MultiGK)
			        device.State.StateClass = XStateClass.Norm;
			}
			foreach (var zone in Zones)
			{
				zone.InitializeInternalState();
			}
		    foreach (var direction in Directions)
		    {
		        direction.InitializeInternalState();
		    }
		    foreach (var pumpStation in PumpStations)
			{
				pumpStation.InitializeInternalState();
			}
			foreach (var mpt in MPTs)
			{
				mpt.InitializeInternalState();
			}
			foreach (var guardZone in GuardZones)
			{
				guardZone.InitializeInternalState();
			}
			foreach (var delay in Delays)
			{
				delay.InitializeInternalState();
			}
			foreach (var door in Doors)
			{
				door.InitializeInternalState();
			}
			foreach (var skdZone in SKDZones)
			{
				skdZone.InitializeInternalState();
			}
			foreach (var code in DeviceConfiguration.Codes)
			{
				code.InitializeInternalState();
			}
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			//foreach (var device in Devices)
			//	if (device.IsRealDevice)
			//	{
			//		var stateClass = device.State.StateClass;
			//		if (stateClass < minStateClass)
			//			minStateClass = device.State.StateClass;
			//	}
			foreach (var zone in Zones)
				if (zone.State != null && zone.State.StateClass < minStateClass)
					minStateClass = zone.State.StateClass;
			foreach (var direction in Directions)
				if (direction.State != null && direction.State.StateClass < minStateClass)
					minStateClass = direction.State.StateClass;
			foreach (var mpt in MPTs)
				if (mpt.State != null && mpt.State.StateClass < minStateClass)
					minStateClass = mpt.State.StateClass;
			foreach (var guardZone in GuardZones)
				if (guardZone.State != null && guardZone.State.StateClass < minStateClass)
					minStateClass = guardZone.State.StateClass;
			return minStateClass;
		}
	}
}