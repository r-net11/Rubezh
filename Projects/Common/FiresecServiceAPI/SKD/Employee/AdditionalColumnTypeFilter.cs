using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AdditionalColumnTypeFilter : OrganizationFilterBase
	{
		[DataMember]
		public List<string> Names { get; set; }

		[DataMember]
		public DataType? Type { get; set; }
	}
}
