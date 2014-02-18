using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class DocumentFilter:OrganizationFilterBase
	{
		[DataMember]
		public List<int> Nos { get; set; }

		[DataMember]
		public DateTimePeriod IssueDate { get; set; }

		[DataMember]
		public DateTimePeriod LaunchDate { get; set; }
	}
}
