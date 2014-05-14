using FiresecAPI.GK;

namespace FiresecClient
{
	public partial class XManager
	{
		public static void CreateStates()
		{
			foreach (var device in Devices)
			{
				device.InternalState = new XDeviceInternalState(device);
				device.State = new XState(device);
			}
			foreach (var zone in Zones)
			{
				zone.InternalState = new XZoneInternalState(zone);
				zone.State = new XState(zone);
			}
			foreach (var direction in Directions)
			{
				direction.InternalState = new XDirectionInternalState(direction);
				direction.State = new XState(direction);
			}
			foreach (var pumpStation in PumpStations)
			{
				pumpStation.InternalState = new XPumpStationInternalState(pumpStation);
				pumpStation.State = new XState(pumpStation);
			}
			foreach (var mpt in MPTs)
			{
				mpt.InternalState = new XMPTInternalState(mpt);
				mpt.State = new XState(mpt);
			}
		}

		public static XStateClass GetMinStateClass()
		{
			var minStateClass = XStateClass.No;
			foreach (var device in XManager.Devices)
				if (device.IsRealDevice)
				{
					var stateClass = device.State.StateClass;
					if (device.DriverType == XDriverType.AM1_T && stateClass == XStateClass.Fire2)
						stateClass = XStateClass.Info;
					if (stateClass < minStateClass)
						minStateClass = device.State.StateClass;
				}
			foreach (var zone in XManager.Zones)
				if (zone.State != null && zone.State.StateClass < minStateClass)
					minStateClass = zone.State.StateClass;
			foreach (var direction in XManager.Directions)
				if (direction.State != null && direction.State.StateClass < minStateClass)
					minStateClass = direction.State.StateClass;
			return minStateClass;
		}
	}
}