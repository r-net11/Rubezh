using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
{
	[DataContract]
	public class Schedule : OrganisationElementBase, IOrganisationElement, IHRListItem
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
		public TimeSpan AllowedLate { get; set; }

		[DataMember]
		public TimeSpan AllowedEarlyLeave { get; set; }

		[DataMember]
		public List<ScheduleZone> Zones { get; set; }

		[DataMember]
		public string Description  { get; set; }

		public string ImageSource { get { return "/Controls;component/Images/Shedule.png"; } }
	}
}