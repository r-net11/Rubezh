using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RubezhDAL.DataClasses
{
	public class EmployeeDay
	{
		[Key]
		public Guid UID { get; set; }
		[Index]
		public Guid? EmployeeUID { get; set; }
		public Employee Employee { get; set; }

		public bool IsIgnoreHoliday { get; set; }

		public bool IsOnlyFirstEnter { get; set; }

		public TimeSpan AllowedLateTimeSpan { get; set; }

		public TimeSpan AllowedEarlyLeaveTimeSpan { get; set; }
		[MaxLength(4000)]
		public string DayIntervalsString { get; set; }

		public DateTime Date { get; set; }
	}
}
