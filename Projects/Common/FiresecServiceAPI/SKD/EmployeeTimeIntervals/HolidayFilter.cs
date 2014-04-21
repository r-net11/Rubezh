using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class HolidayFilter : OrganizationFilterBase
	{
		public HolidayFilter()
		{
		}

		[DataMember]
		public int Year { get; set; }
	}
}