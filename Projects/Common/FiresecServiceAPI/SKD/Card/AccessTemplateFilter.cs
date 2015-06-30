using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class AccessTemplateFilter : OrganisationFilterBase
	{
		[DataMember]
		public List<string> Names { get; set; }
	}
}