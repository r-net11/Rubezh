using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class TimeIntervalFilter : IsDeletedFilter
	{
		public TimeIntervalFilter()
		{
		}

		[DataMember]
		public List<Guid> NamedIntervalUIDs { get; set; }
	}
}