using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class EmployeeDay
	{
		public EmployeeDay()
		{
			UID = Guid.NewGuid();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public bool IsIgnoreHoliday { get; set; }

		[DataMember]
		public bool IsOnlyFirstEnter { get; set; }

		[DataMember]
		public int AllowedLate { get; set; }

		[DataMember]
		public int AllowedEarlyLeave { get; set; }

		[DataMember]
		public string DayIntervalsString { get; set; }

		[DataMember]
		public DateTime Date { get; set; }
	}
}