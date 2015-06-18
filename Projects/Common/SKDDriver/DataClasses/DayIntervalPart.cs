using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class DayIntervalPart
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? DayIntervalUID { get; set; }
		public DayInterval DayInterval { get; set; }

		public int BeginTime { get; set; }

		public int EndTime { get; set; }
	}
}
