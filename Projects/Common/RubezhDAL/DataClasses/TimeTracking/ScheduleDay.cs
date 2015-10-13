using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class ScheduleDay
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? DayIntervalUID { get; set; }
		public DayInterval DayInterval { get; set; }
		[Index]
		public Guid? ScheduleSchemeUID { get; set; }
		public ScheduleScheme ScheduleScheme { get; set; }

		public int Number { get; set; }
	}
}
