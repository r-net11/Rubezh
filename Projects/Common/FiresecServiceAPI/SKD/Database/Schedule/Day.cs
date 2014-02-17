using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class Day : OrganizationElementBase
	{
		[DataMember]
		public Guid NamedIntervalUid { get; set; }

		[DataMember]
		public int? Number { get; set; }
	}
}