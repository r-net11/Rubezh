using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class HolidayFilter : OrganisationFilterBase
	{
		[DataMember]
		public int Year { get; set; }
	}
}