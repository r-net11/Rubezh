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
		}

		[DataMember]
		public List<Guid> ZoneUIDs { get; set; }

		[DataMember]
		public List<Guid> GuardZoneUIDs { get; set; }
	}
}