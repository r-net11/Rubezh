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
	}
}