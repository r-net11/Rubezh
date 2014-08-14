using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleDayIntervalFilter : IsDeletedFilter
	{
		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}