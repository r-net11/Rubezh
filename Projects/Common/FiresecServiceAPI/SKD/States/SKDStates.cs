using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class SKDStates
	{
		public SKDStates()
		{
			DeviceStates = new List<SKDDeviceState>();
			ZoneStates = new List<SKDZoneState>();
		}

		[DataMember]
		public List<SKDDeviceState> DeviceStates { get; set; }

		[DataMember]
		public List<SKDZoneState> ZoneStates { get; set; }
	}
}