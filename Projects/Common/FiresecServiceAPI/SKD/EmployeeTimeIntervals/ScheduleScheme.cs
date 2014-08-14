using System.Collections.Generic;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleScheme : OrganisationElementBase
	{
		public ScheduleScheme()
		{
			DayIntervals = new List<ScheduleDayInterval>();
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ScheduleSchemeType Type { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public List<ScheduleDayInterval> DayIntervals { get; set; }
	}
}