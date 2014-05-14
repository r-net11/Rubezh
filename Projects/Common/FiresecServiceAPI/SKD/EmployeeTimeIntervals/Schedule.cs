using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class Schedule : OrganisationElementBase
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