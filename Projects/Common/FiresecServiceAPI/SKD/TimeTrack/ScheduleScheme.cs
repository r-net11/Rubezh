using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class ScheduleScheme : OrganisationElementBase, IOrganisationElement
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

		[DataMember]
		public int DaysCount { get; set; }
	}
}