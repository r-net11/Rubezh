using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleSchemeFilter : OrganisationFilterBase
	{
		public ScheduleSchemeFilter()
		{
			WithDays = true;
		}

		[DataMember]
		public ScheduleSchemeType Type { get; set; }

		[DataMember]
		public bool WithDays { get; set; }
	}
}