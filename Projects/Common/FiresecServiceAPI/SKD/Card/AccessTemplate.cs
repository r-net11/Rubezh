using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
    public class AccessTemplate : OrganisationElementBase, IWithName
	{
		public AccessTemplate()
			: base()
		{
			CardDoors = new List<CardDoor>();
			GuardZoneAccesses = new List<XGuardZoneAccess>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<CardDoor> CardDoors { get; set; }

		[DataMember]
		public List<XGuardZoneAccess> GuardZoneAccesses { get; set; }
	}
}