using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class GKStates
	{
		public GKStates()
		{
			DeviceStates = new List<XState>();
			ZoneStates = new List<XState>();
			DirectionStates = new List<XState>();
			PumpStationStates = new List<XState>();
		}

		[DataMember]
		public List<XState> DeviceStates { get; set; }

		[DataMember]
		public List<XState> ZoneStates { get; set; }

		[DataMember]
		public List<XState> DirectionStates { get; set; }

		[DataMember]
		public List<XState> PumpStationStates { get; set; }
	}
}