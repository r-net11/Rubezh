using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
 {
	[DataContract]
	public class DocumentFilter:OrganizationFilterBase
	{
		[DataMember]
		public List<int> Nos { get; set; }

		[DataMember]
		public List<string> Names { get; set; }

		[DataMember]
		public DateTimePeriod IssueDate { get; set; }

		[DataMember]
		public DateTimePeriod LaunchDate { get; set; }

		public DocumentFilter()
			:base()
		{
			Nos = new List<int>();
			Names = new List<string>();
			IssueDate = new DateTimePeriod();
			LaunchDate = new DateTimePeriod();
		}
	}
}
