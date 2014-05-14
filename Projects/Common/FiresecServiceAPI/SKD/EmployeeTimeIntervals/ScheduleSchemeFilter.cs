using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
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