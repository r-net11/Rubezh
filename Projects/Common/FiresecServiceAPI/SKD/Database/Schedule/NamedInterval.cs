using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class NamedInterval : OrganizationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<Guid> IntervalUids { get; set; }
	}
}