using System;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class ScheduleZone : SKDIsDeletedModel
	{
		public ScheduleZone()
		{
		}

		[DataMember]
		public Guid ScheduleUID { get; set; }

		[DataMember]
		public Guid ZoneUID { get; set; }

		[DataMember]
		public bool IsControl { get; set; }
	}
}