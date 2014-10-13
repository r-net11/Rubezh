using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	public class GKHashConfiguration : VersionedConfiguration
	{
		public GKHashConfiguration()
		{
		}

		public GKHashConfiguration(GKDeviceConfiguration deviceConfiguration)
		{
			RootDevice = deviceConfiguration.RootDevice;
			Devices = deviceConfiguration.Devices;
			Zones = deviceConfiguration.Zones;
			GuardZones = deviceConfiguration.GuardZones;
			Directions = deviceConfiguration.Directions;
			PumpStations = deviceConfiguration.PumpStations;
			Doors = deviceConfiguration.Doors;
		}

		public GKDevice RootDevice { get; set; }
		public List<GKDevice> Devices { get; set; }
		public List<GKZone> Zones { get; set; }
		public List<GKGuardZone> GuardZones { get; set; }
		public List<GKDirection> Directions { get; set; }
		public List<GKPumpStation> PumpStations { get; set; }
		public List<GKDoor> Doors { get; set; }
	}
}