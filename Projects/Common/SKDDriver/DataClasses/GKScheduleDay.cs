using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class GKScheduleDay
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? ScheduleUID { get; set; }
		public GKSchedule Schedule { get; set; }

		public DateTime DateTime { get; set; }
	}
}
