using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class NamedIntervalFilter : OrganisationFilterBase
	{
		public NamedIntervalFilter()
		{
		}
	}
}