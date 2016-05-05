using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ScheduleSchemeFilter : OrganisationFilterBase
	{
		public ScheduleSchemeFilter()
		{
			WithDays = true;
			DayIntervalUIDs = new List<Guid>();
		}

		[DataMember]
		public ScheduleSchemeType Type { get; set; }

		[DataMember]
		public bool WithDays { get; set; }

		[DataMember]
		public List<Guid> DayIntervalUIDs { get; set; }
	}
}