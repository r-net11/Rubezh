using System;
using System.Runtime.Serialization;
using FiresecAPI.SKD;

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
	}
}