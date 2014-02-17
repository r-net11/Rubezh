using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeFilter : OrganizationFilterBase
	{
		[DataMember]
		public List<Guid> PositionUids { get; set; }

		[DataMember]
		public List<Guid> DepartmentUids { get; set; }

		[DataMember]
		public DateTimePeriod Appointed { get; set; }

		[DataMember]
		public DateTimePeriod Dismissed { get; set; }

		public EmployeeFilter():base()
		{
			PositionUids = new List<Guid>();
			DepartmentUids = new List<Guid>();
		}
	}
}