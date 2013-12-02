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
			DeviceStates = new List<XDeviceState>();
			ZoneStates = new List<XZoneState>();
			DirectionStates = new List<XDirectionState>();
		}

		[DataMember]
		public List<XDeviceState> DeviceStates { get; set; }

		[DataMember]
		public List<XZoneState> ZoneStates { get; set; }

		[DataMember]
		public List<XDirectionState> DirectionStates { get; set; }
	}
}