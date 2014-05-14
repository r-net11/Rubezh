using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Day : OrganisationElementBase
	{
		[DataMember]
		public Guid NamedIntervalUid { get; set; }

		[DataMember]
		public int? Number { get; set; }
	}
}