using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class AccessTemplate : OrganisationElementBase
	{
		public AccessTemplate()
			: base()
		{
			CardZones = new List<CardZone>();
			GuardZoneAccesses = new List<XGuardZoneAccess>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<CardZone> CardZones { get; set; }

		[DataMember]
		public List<XGuardZoneAccess> GuardZoneAccesses { get; set; }
	}
}