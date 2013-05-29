using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XPumpStationProperty
	{
		public XPumpStationProperty()
		{
            PumpStationPumps = new List<XPumpStationPump>();
			DirectionUIDs = new List<Guid>();
			PumpsCount = 1;
			DelayTime = 2;
		}

		[DataMember]
		public List<Guid> DirectionUIDs { get; set; }

        [DataMember]
        public List<XPumpStationPump> PumpStationPumps { get; set; }

		[DataMember]
		public ushort PumpsCount { get; set; }

		[DataMember]
		public ushort DelayTime { get; set; }
	}
}