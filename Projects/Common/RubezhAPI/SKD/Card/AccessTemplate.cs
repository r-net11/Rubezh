using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class AccessTemplate : OrganisationElementBase, IOrganisationElement, IHRListItem
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

		public string ImageSource { get { return "/Controls;component/Images/AccessTemplate.png"; } }
	}
}