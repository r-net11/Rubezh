using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class EmployeeTimeTrack
	{
		public EmployeeTimeTrack()
		{
			EmployeeTimeTrackParts = new List<EmployeeTimeTrackPart>();
		}

		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public DateTime Date { get; set; }

		[DataMember]
		public List<EmployeeTimeTrackPart> EmployeeTimeTrackParts { get; set; }

		[DataMember]
		public DateTime Total { get; set; }

		[DataMember]
		public DateTime TotalMiss { get; set; }

		[DataMember]
		public DateTime TotalInSchedule { get; set; }

		[DataMember]
		public DateTime TotalOutSchedule { get; set; }
	}

	[DataContract]
	public class EmployeeTimeTrackPart
	{
		[DataMember]
		public DateTime StartTime { get; set; }

		[DataMember]
		public DateTime EndTime { get; set; }
	}
}