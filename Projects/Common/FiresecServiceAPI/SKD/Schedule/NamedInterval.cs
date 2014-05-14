using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class NamedInterval : OrganisationElementBase
	{
		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public List<Guid> IntervalUids { get; set; }
	}
}