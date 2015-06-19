using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKStates
	{
		public GKStates()
		{
			DeviceStates = new List<GKState>();
			ZoneStates = new List<GKState>();
			PimStates = new List<GKState>();
			DoorStates = new List<GKState>();
			SKDZoneStates = new List<GKState>();
			DeviceMeasureParameters = new List<GKDeviceMeasureParameters>();
		}

		[DataMember]
		public List<GKState> DeviceStates { get; set; }

		[DataMember]
		public List<GKState> ZoneStates { get; set; }

		[DataMember]
		public List<GKState> PimStates { get; set; }

		[DataMember]
		public List<GKState> DoorStates { get; set; }

		[DataMember]
		public List<GKState> SKDZoneStates { get; set; }

		[DataMember]
		public List<GKDeviceMeasureParameters> DeviceMeasureParameters { get; set; }
	}
}