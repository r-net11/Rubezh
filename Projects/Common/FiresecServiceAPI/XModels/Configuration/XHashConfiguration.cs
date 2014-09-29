using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	public class XHashConfiguration : VersionedConfiguration
	{
		public XHashConfiguration(XDeviceConfiguration deviceConfiguration)
		{
			RootDevice = deviceConfiguration.RootDevice;
			Devices = deviceConfiguration.Devices;
			Zones = deviceConfiguration.Zones;
			GuardZones = deviceConfiguration.GuardZones;
			Directions = deviceConfiguration.Directions;
			PumpStations = deviceConfiguration.PumpStations;
		}

		public XDevice RootDevice { get; set; }
		public List<XDevice> Devices { get; set; }
		public List<XZone> Zones { get; set; }
		public List<XGuardZone> GuardZones { get; set; }
		public List<XDirection> Directions { get; set; }
		public List<XPumpStation> PumpStations { get; set; }
	}
}