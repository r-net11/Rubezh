using System.Collections.Generic;
using System.Runtime.Serialization;
using System;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumnTypeFilter : OrganisationFilterBase
	{
		[DataMember]
		public AdditionalColumnDataType? Type { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; }
	}
}