using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class AccessTemplateFilter : OrganisationFilterBase
	{
		[DataMember]
		public List<string> Names { get; set; }
	}
}