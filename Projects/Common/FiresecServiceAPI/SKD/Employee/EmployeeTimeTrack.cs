using System;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class EmployeeTimeTrack
	{
		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public DateTime Date { get; set; }

		[DataMember]
		public DateTime Total { get; set; }

		[DataMember]
		public DateTime TotalMiss { get; set; }

		[DataMember]
		public DateTime TotalInSchedule { get; set; }

		[DataMember]
		public DateTime TotalOutSchedule { get; set; }
}
}
