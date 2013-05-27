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
			FirePumpUIDs = new List<Guid>();
			DirectionUIDs = new List<Guid>();
			PumpsCount = 1;
			DelayTime = 2;
		}

		[DataMember]
		public List<Guid> DirectionUIDs { get; set; }

        public List<XPumpStationPump> PumpStationPumps { get; set; }

		[DataMember]
		public List<Guid> FirePumpUIDs { get; set; }

		[DataMember]
		public Guid JokeyPumpUID { get; set; }

		[DataMember]
		public Guid CompressorPumpUID { get; set; }

		[DataMember]
		public Guid DrenajPumpUID { get; set; }

		[DataMember]
		public Guid CompensationPumpUID { get; set; }

		[DataMember]
		public ushort PumpsCount { get; set; }

		[DataMember]
		public ushort DelayTime { get; set; }
	}
}