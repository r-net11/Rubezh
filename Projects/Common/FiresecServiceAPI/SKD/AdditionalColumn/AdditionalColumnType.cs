using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumnType : OrganizationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public AdditionalColumnDataType DataType { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; }
	}
}