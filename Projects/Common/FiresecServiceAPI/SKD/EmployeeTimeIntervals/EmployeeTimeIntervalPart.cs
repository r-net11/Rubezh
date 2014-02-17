using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeTimeIntervalPart
	{
		public EmployeeTimeIntervalPart()
		{
			UID = Guid.NewGuid();
			StartTime = new DateTime(2000, 1, 1);
			EndTime = new DateTime(2000, 1, 1);
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public DateTime EndTime { get; set; }

		[DataMember]
		public IntervalTransitionType IntervalTransitionType { get; set; }
	}
}