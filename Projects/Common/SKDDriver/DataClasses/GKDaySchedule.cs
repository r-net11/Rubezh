using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class GKDaySchedule
	{
		[Key]
		public Guid UID { get; set; }

		public ICollection<ScheduleGKDaySchedule> ScheduleGKDaySchedules { get; set; }

		public ICollection<GKDaySchedulePart> GKDayScheduleParts { get; set; }

		public int No { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }
	}
}
