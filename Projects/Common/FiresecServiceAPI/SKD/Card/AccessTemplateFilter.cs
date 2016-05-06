using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class AccessTemplateFilter : OrganisationFilterBase
	{
		[DataMember]
		public List<string> Names { get; set; }
	}
}