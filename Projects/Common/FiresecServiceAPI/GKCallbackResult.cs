using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class GKCallbackResult
	{
		[DataMember]
		public List<JournalItem> JournalItems { get; private set; }

		[DataMember]
		public List<XDeviceState> DeviceStates { get; private set; }

		[DataMember]
		public List<XZoneState> ZoneStates { get; private set; }

		[DataMember]
		public List<XDirectionState> DirectionStates { get; private set; }

		public GKCallbackResult()
		{
			JournalItems = new List<JournalItem>();
			DeviceStates = new List<XDeviceState>();
			ZoneStates = new List<XZoneState>();
			DirectionStates = new List<XDirectionState>();
		}
	}
}