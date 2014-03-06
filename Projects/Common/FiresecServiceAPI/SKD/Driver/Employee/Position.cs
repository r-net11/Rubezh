using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Position : OrganizationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}