using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

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