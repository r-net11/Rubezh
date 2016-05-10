using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class AccessTemplate : OrganisationElementBase, IOrganisationElement
	{
		public AccessTemplate()
			: base()
		{
			CardDoors = new List<CardDoor>();
			DeactivatingReaders = new List<AccessTemplateDeactivatingReader>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<CardDoor> CardDoors { get; set; }

		[DataMember]
		public List<AccessTemplateDeactivatingReader> DeactivatingReaders { get; set; }
	}
}