using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class SKDStates
	{
		public SKDStates()
		{
			DeviceStates = new List<SKDDeviceState>();
			ZoneStates = new List<SKDZoneState>();
			DoorStates = new List<SKDDoorState>();
		}

		[DataMember]
		public List<SKDDeviceState> DeviceStates { get; set; }

		[DataMember]
		public List<SKDZoneState> ZoneStates { get; set; }

		[DataMember]
		public List<SKDDoorState> DoorStates { get; set; }
	}
}