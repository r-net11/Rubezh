using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Position : OrganisationElementBase, IOrganisationElement
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Photo Photo { get; set; }
	}
}