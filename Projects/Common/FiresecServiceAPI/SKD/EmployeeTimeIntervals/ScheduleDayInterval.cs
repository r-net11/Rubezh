using System;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleDayInterval : SKDIsDeletedModel
	{
		[DataMember]
		public int Number { get; set; }

		[DataMember]
		public Guid DayIntervalUID { get; set; }

		[DataMember]
		public Guid ScheduleSchemeUID { get; set; }
	}
}