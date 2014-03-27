using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeFilter : OrganizationFilterBase
	{
		public EmployeeFilter()
			: base()
		{
			PositionUIDs = new List<Guid>();
			DepartmentUIDs = new List<Guid>();
			Appointed = new DateTimePeriod();
			Dismissed = new DateTimePeriod();
		}

		[DataMember]
		public List<Guid> PositionUIDs { get; set; }

		[DataMember]
		public List<Guid> DepartmentUIDs { get; set; }

		[DataMember]
		public Guid OrganisationUID { get; set; }

		[DataMember]
		public DateTimePeriod Appointed { get; set; }

		[DataMember]
		public DateTimePeriod Dismissed { get; set; }
	}
}