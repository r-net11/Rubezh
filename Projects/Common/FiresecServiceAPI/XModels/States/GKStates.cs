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
			DirectionStates = new List<GKState>();
			PumpStationStates = new List<GKState>();
			MPTStates = new List<GKState>();
			DelayStates = new List<GKState>();
			PimStates = new List<GKState>();
			GuardZoneStates = new List<GKState>();
			DeviceMeasureParameters = new List<GKDeviceMeasureParameters>();
		}

		[DataMember]
		public List<GKState> DeviceStates { get; set; }

		[DataMember]
		public List<GKState> ZoneStates { get; set; }

		[DataMember]
		public List<GKState> DirectionStates { get; set; }

		[DataMember]
		public List<GKState> PumpStationStates { get; set; }

		[DataMember]
		public List<GKState> MPTStates { get; set; }

		[DataMember]
		public List<GKState> DelayStates { get; set; }

		[DataMember]
		public List<GKState> PimStates { get; set; }

		[DataMember]
		public List<GKState> GuardZoneStates { get; set; }

		[DataMember]
		public List<GKDeviceMeasureParameters> DeviceMeasureParameters { get; set; }
	}
}