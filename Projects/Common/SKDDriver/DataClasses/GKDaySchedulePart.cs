using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class GKDaySchedulePart
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? DayScheduleUID { get; set; }
		public GKDaySchedule DaySchedule { get; set; }

		public int No { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public int StartMilliseconds { get; set; }

		public int EndMilliseconds { get; set; }
	}
}
