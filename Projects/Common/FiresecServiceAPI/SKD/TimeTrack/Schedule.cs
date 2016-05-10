using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.SKD
{
	[DataContract]
	public class Schedule : OrganisationElementBase, IOrganisationElement
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
		public int AllowedLate { get; set; }

		[DataMember]
		public bool IsEnabledAllowLate { get; set; }

		[DataMember]
		public int AllowedAbsentLowThan { get; set; }

		[DataMember]
		public bool IsAllowAbsent { get; set; }

		[DataMember]
		public int NotAllowOvertimeLowerThan { get; set; }

		[DataMember]
		public bool IsEnabledOvertime { get; set; }

		[DataMember]
		public int AllowedEarlyLeave { get; set; }

		[DataMember]
		public bool IsEnabledAllowEarlyLeave { get; set; }

		[DataMember]
		public List<ScheduleZone> Zones { get; set; }

		public string Description
		{
			get { return ""; }
			set { return; }
		}
	}
}