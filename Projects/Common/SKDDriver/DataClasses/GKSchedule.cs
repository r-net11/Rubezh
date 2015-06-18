using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SKDDriver.DataClasses
{
	public class GKSchedule
	{
		public GKSchedule()
		{
			ScheduleDays = new List<GKScheduleDay>();
			ScheduleGKDaySchedules = new List<ScheduleGKDaySchedule>();
		}
		
		[Key]
		public Guid UID { get; set; }

		public ICollection<GKScheduleDay> ScheduleDays { get; set; }

		public ICollection<ScheduleGKDaySchedule> ScheduleGKDaySchedules { get; set; }

		public int No { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public int Type { get; set; }

		public int PeriodType { get; set; }

		public DateTime StartDateTime { get; set; }

		public int HoursPeriod { get; set; }

		public int HolidayScheduleNo { get; set; }

		public int WorkingHolidayScheduleNo { get; set; }

		public int Year { get; set; }
	}
}
