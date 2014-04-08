using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class Schedule : OrganizationElementBase
	{
		public Schedule()
		{
			Zones = new List<ScheduleZone>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }

		[DataMember]
		public bool IsIgnoreHoliday { get; set; }

		[DataMember]
		public bool IsOnlyFirstEnter { get; set; }

		[DataMember]
		public List<ScheduleZone> Zones { get; set; }
	}
}