using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;

namespace RubezhAPI.OPC
{
	[DataContract]
	public class OPCSettings : ModelBase
	{
		public OPCSettings()
		{
			ZoneUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
			DeviceUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();
			DiretionUIDs = new List<Guid>();
			NSUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
			DoorUIDs = new List<Guid>();
		}

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> DeviceUIDs { get; set; }

		[DataMember]
		public List<Guid> DelayUIDs { get; set; }

		[DataMember]
		public List<Guid> DiretionUIDs { get; set; }

		[DataMember]
		public List<Guid> NSUIDs { get; set; }

		[DataMember]
		public List<Guid> MPTUIDs { get; set; }

		[DataMember]
		public List<Guid> DoorUIDs { get; set; }
 
	}
}
