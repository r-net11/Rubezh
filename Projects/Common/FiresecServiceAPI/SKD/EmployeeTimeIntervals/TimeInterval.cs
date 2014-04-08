using System;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class TimeInterval : SKDIsDeletedModel
	{
		public TimeInterval()
		{
			StartTime = new DateTime(2000, 1, 1);
			EndTime = new DateTime(2000, 1, 1);
		}

		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public DateTime EndTime { get; set; }

		[DataMember]
		public IntervalTransitionType IntervalTransitionType { get; set; }
	}
}