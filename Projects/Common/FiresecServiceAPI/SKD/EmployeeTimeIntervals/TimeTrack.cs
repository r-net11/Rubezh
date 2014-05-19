using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD.EmployeeTimeIntervals
{
	[DataContract]
	public class TimeTrack
	{
		[DataMember]
		public Guid EmployeeUID { get; set; }

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }

		[DataMember]
		public string PositionName { get; set; }

		[DataMember]
		public List<double> Hours { get; set; }
	}
}
