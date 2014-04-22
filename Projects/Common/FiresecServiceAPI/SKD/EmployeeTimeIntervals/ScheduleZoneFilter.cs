using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleZoneFilter : IsDeletedFilter
	{
		public ScheduleZoneFilter()
		{
		}

		[DataMember]
		public List<Guid> ScheduleUIDs { get; set; }
	}
}