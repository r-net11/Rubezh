using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class DocumentFilter : OrganisationFilterBase
	{
		public DocumentFilter()
			: base()
		{
			IssueDate = new DateTimePeriod();
			LaunchDate = new DateTimePeriod();
		}

		[DataMember]
		public DateTimePeriod IssueDate { get; set; }

		[DataMember]
		public DateTimePeriod LaunchDate { get; set; }
	}
}