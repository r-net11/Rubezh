using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleSchemeFilter : OrganizationFilterBase
	{
		public ScheduleSchemeFilter()
		{
		}

		[DataMember]
		public ScheduleSchemeType Type { get; set; }
	}
}