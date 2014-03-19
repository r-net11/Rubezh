using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AccessTemplateFilter : OrganizationFilterBase
	{
		[DataMember]
		public List<string> Names { get; set; }
	}
}