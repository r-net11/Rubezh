using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class GKDaySchedulePart
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? DayScheduleUID { get; set; }
		public GKDaySchedule DaySchedule { get; set; }

		public int No { get; set; }
		[MaxLength(50)]
		public string Name { get; set; }
		[MaxLength(50)]
		public string Description { get; set; }

		public int StartMilliseconds { get; set; }

		public int EndMilliseconds { get; set; }
	}
}
