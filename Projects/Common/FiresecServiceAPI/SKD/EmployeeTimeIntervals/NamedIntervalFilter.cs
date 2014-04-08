using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class NamedIntervalFilter : OrganizationFilterBase
	{
		public NamedIntervalFilter()
		{
		}
	}
}