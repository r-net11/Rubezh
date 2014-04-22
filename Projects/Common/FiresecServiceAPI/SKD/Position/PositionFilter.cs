using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class PositionFilter : OrganisationFilterBase
	{
		public List<string> Names { get; set; }
	}
}