using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class HolidayFilter : OrganisationFilterBase
	{
		public HolidayFilter()
		{
		}

		[DataMember]
		public int Year { get; set; }
	}
}