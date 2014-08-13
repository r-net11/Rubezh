using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class DayIntervalFilter : IsDeletedFilter
	{
		[DataMember]
		public List<Guid> ScheduleSchemeUIDs { get; set; }
	}
}