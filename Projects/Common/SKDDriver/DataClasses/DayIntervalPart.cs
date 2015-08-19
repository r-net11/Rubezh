using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SKDDriver.DataClasses
{
	public class DayIntervalPart
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? DayIntervalUID { get; set; }
		public DayInterval DayInterval { get; set; }

		public TimeSpan BeginTime { get; set; }

		public TimeSpan EndTime { get; set; }

		public int Number { get; set; }
	}
}
