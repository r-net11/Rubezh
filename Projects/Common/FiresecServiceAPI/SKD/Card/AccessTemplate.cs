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
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<CardDoor> CardDoors { get; set; }
	}
}