using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class ScheduleZone
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? ScheduleUID { get; set; }
		public Schedule Schedule { get; set; }

		public Guid ZoneUID { get; set; }

		public Guid DoorUID { get; set; }
	}
}
