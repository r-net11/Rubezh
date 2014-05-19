using System.Runtime.Serialization;
using FiresecAPI.SKD;

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