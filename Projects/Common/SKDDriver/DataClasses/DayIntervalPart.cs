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

		public int BeginTime { get; set; }

		public int EndTime { get; set; }
	}
}
