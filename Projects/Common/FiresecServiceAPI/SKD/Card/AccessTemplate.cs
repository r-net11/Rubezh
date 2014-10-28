using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class AccessTemplate : OrganisationElementBase, IOrganisationElement
	{
		public AccessTemplate()
			: base()
		{
			CardDoors = new List<CardDoor>();
			GKCardDoors = new List<GKCardDoor>();
			GuardZoneAccesses = new List<GKGuardZoneAccess>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<CardDoor> CardDoors { get; set; }

		[DataMember]
		public List<GKCardDoor> GKCardDoors { get; set; }

		[DataMember]
		public List<GKGuardZoneAccess> GuardZoneAccesses { get; set; }
	}
}