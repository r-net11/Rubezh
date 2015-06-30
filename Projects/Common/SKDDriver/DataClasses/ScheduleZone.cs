using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class ScheduleZone
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? ScheduleUID { get; set; }
		public Schedule Schedule { get; set; }

		public Guid ZoneUID { get; set; }

		public Guid DoorUID { get; set; }
	}
}
