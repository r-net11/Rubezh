using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleScheme : OrganisationElementBase
	{
		public ScheduleScheme()
		{
			DayIntervals = new List<DayInterval>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ScheduleSchemeType Type { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<DayInterval> DayIntervals { get; set; }
	}
}
