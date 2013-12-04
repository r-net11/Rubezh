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
		public List<XState> DeviceStates { get; private set; }

		[DataMember]
		public List<XState> ZoneStates { get; private set; }

		[DataMember]
		public List<XState> DirectionStates { get; private set; }

		[DataMember]
		public List<XState> PumpStationStates { get; private set; }

		public GKCallbackResult()
		{
			JournalItems = new List<JournalItem>();
			DeviceStates = new List<XState>();
			ZoneStates = new List<XState>();
			DirectionStates = new List<XState>();
			PumpStationStates = new List<XState>();
		}
	}
}