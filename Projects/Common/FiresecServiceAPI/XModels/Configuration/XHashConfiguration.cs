using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XHashConfiguration : VersionedConfiguration
	{
		public XHashConfiguration(XDeviceConfiguration deviceConfiguration)
		{
			RootDevice = deviceConfiguration.RootDevice;
			Devices = deviceConfiguration.Devices;
			Zones = deviceConfiguration.Zones;
			Directions = deviceConfiguration.Directions;
			PumpStations = deviceConfiguration.PumpStations;
		}

		[DataMember]
		public XDevice RootDevice { get; set; }

		[DataMember]
		public List<XDevice> Devices { get; set; }

		[DataMember]
		public List<XZone> Zones { get; set; }

		[DataMember]
		public List<XDirection> Directions { get; set; }

		[DataMember]
		public List<XPumpStation> PumpStations { get; set; }

	}
}