using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class PositionFilter : OrganizationFilterBase
	{
		public List<string> Names { get; set; }
	}
}