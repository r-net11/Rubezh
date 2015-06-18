using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class Schedule : IOrganisationItem
	{
		public Schedule()
		{
			ScheduleZones = new List<ScheduleZone>();
		}
		
		#region IOrganisationItemMembers
		[Key]
		public Guid UID { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? RemovalDate { get; set; }

		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		#endregion

		public Guid? ScheduleSchemeUID { get; set; }
		public ScheduleScheme ScheduleScheme { get; set; }

		public ICollection<ScheduleZone> ScheduleZones { get; set; }

		public bool IsIgnoreHoliday { get; set; }

		public bool IsOnlyFirstEnter { get; set; }

		public int AllowedLate { get; set; }

		public int AllowedEarlyLeave { get; set; }
	}
}
