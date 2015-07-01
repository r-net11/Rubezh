using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKReflectionItem
	{
		public GKReflectionItem()
		{
			ZoneUIDs = new List<Guid>();
			GuardZoneUIDs = new List<Guid>();
			DeviceUIDs = new List<Guid>();
			DelayUIDs = new List<Guid>();
			DiretionUIDs = new List<Guid>();
			NSUIDs = new List<Guid>();
			MPTUIDs = new List<Guid>();
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
	}
}