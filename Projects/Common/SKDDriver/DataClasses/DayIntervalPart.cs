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

		public DateTime BeginTime { get; set; }

		public DateTime EndTime { get; set; }
	}
}
