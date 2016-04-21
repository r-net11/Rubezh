using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class ScheduleScheme : OrganisationElementBase, IOrganisationElement, IHRListItem
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

		public string ImageSource { get { return "/Controls;component/Images/Month.png"; } }
	}
}