using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RubezhDAL.DataClasses
{
	public class GKDaySchedule
	{
		public GKDaySchedule()
		{
			ScheduleGKDaySchedules = new List<ScheduleGKDaySchedule>();
			GKDayScheduleParts = new List<GKDaySchedulePart>();
		}
		
		[Key]
		public Guid UID { get; set; }

		public ICollection<ScheduleGKDaySchedule> ScheduleGKDaySchedules { get; set; }

		public ICollection<GKDaySchedulePart> GKDayScheduleParts { get; set; }

		public int No { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(50)]
		public string Description { get; set; }
	}
}
