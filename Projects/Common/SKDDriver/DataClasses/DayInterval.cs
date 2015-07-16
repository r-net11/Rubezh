using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class DayInterval : IOrganisationItem
	{
		public DayInterval()
		{
			ScheduleDays = new List<ScheduleDay>();
			DayIntervalParts = new List<DayIntervalPart>();
		}

		#region IOrganisationItemMembers
		[Key]
		public Guid UID { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(4000)]
		public string Description { get; set; }

		public bool IsDeleted { get; set; }

		public DateTime? RemovalDate { get; set; }
		[Index]
		public Guid? OrganisationUID { get; set; }
		public Organisation Organisation { get; set; }
		#endregion

		public int SlideTime { get; set; }

		public ICollection<ScheduleDay> ScheduleDays { get; set; }

		public ICollection<DayIntervalPart> DayIntervalParts { get; set; }
	}
}
