using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class ScheduleSchemeFilter : OrganisationFilterBase
	{
		public ScheduleSchemeFilter()
		{
			WithDays = true;
		}

		[DataMember]
		public ScheduleSchemeType Type { get; set; }

		[DataMember]
		public bool WithDays { get; set; }
	}
}