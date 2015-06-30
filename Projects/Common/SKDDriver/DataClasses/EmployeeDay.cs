using System;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class EmployeeDay
	{
		[Key]
		public Guid UID { get; set; }

		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public bool IsIgnoreHoliday { get; set; }

		public bool IsOnlyFirstEnter { get; set; }

		public int AllowedLate { get; set; }

		public int AllowedEarlyLeave { get; set; }

		public string DayIntervalsString { get; set; }

		public DateTime Date { get; set; }
	}
}
