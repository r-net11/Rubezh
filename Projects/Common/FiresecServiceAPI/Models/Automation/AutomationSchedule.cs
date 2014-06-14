using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class AutomationSchedule
	{
		public AutomationSchedule()
		{
			Name = "Новое расписание";
			Uid = Guid.NewGuid();
			Year = -1;
			Month = -1;
			Day = -1;
			Hour = -1;
			Minute = -1;
			Second = -1;
			DayOfWeek = DayOfWeekType.Any;
		}

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
		public DayOfWeekType DayOfWeek { get; set; }

		[DataMember]
		public int Period { get; set; }

		[DataMember]
		public bool IsPeriodSelected { get; set; }
	}
}