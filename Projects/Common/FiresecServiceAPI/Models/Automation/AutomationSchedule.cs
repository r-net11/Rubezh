using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class AutomationSchedule
	{
		public AutomationSchedule()
		{
			Name = Resources.Language.Models.Automation.AutomationSchedule.Name;
			Uid = Guid.NewGuid();
			Year = -1;
			Month = -1;
			Day = -1;
			Hour = -1;
			Minute = -1;
			Second = -1;
			PeriodDay = 0;
			PeriodHour = 0;
			PeriodMinute = 0;
			PeriodSecond = 1;
			ScheduleProcedures = new List<ScheduleProcedure>();
			DayOfWeek = DayOfWeekType.Any;
		}

		[DataMember]
		public List<ScheduleProcedure> ScheduleProcedures { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public int Year { get; set; }

		[DataMember]
		public int Month { get; set; }

		[DataMember]
		public int Day { get; set; }

		[DataMember]
		public int Hour { get; set; }

		[DataMember]
		public int Minute { get; set; }

		[DataMember]
		public int Second { get; set; }

		[DataMember]
		public int PeriodDay { get; set; }

		[DataMember]
		public int PeriodHour { get; set; }

		[DataMember]
		public int PeriodMinute { get; set; }

		[DataMember]
		public int PeriodSecond { get; set; }

		[DataMember]
		public DayOfWeekType DayOfWeek { get; set; }

		[DataMember]
		public bool IsPeriodSelected { get; set; }

		[DataMember]
		public bool IsActive { get; set; }
	}
}