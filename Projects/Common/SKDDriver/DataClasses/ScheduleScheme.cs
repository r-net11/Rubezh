using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class ScheduleScheme : IOrganisationItem
	{
		public ScheduleScheme()
		{
			Schedules = new List<Schedule>();
			ScheduleDays = new List<ScheduleDay>();
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

		public ICollection<Schedule> Schedules { get; set; }

		public ICollection<ScheduleDay> ScheduleDays { get; set; }

		public int Type { get; set; }

		public int DaysCount { get; set; }
	}
}
