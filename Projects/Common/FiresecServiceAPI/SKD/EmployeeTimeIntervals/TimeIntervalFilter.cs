using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

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