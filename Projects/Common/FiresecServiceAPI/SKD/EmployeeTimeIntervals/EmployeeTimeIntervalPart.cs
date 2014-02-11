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