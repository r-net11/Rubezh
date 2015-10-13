using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class GKScheduleDay
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? ScheduleUID { get; set; }
		public GKSchedule Schedule { get; set; }

		public DateTime DateTime { get; set; }
	}
}
