using System;
using System.Runtime.Serialization;

namespace FiresecAPI.EmployeeTimeIntervals
{
	[DataContract]
	public class TimeInterval : SKDIsDeletedModel
	{
		public TimeInterval()
		{
			BeginTime = new TimeSpan(0,0,0);
			EndTime = new TimeSpan(0, 0, 0);
		}

		[DataMember]
		public Guid NamedIntervalUID { get; set; }

		[DataMember]
		public TimeSpan BeginTime { get; set; }

		[DataMember]
		public TimeSpan EndTime { get; set; }

		[DataMember]
		public IntervalTransitionType IntervalTransitionType { get; set; }
	}
}