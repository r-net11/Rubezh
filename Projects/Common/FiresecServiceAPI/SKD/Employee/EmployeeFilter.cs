using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeFilter : OrganisationFilterBase
	{
		public EmployeeFilter()
			: base()
		{
			DepartmentUIDs = new List<Guid>();
			PositionUIDs = new List<Guid>();
			Appointed = new DateTimePeriod();
			Dismissed = new DateTimePeriod();
			PersonType = PersonType.Employee;
		}

		[DataMember]
		public List<Guid> DepartmentUIDs { get; set; }

		[DataMember]
		public List<Guid> PositionUIDs { get; set; }

		[DataMember]
		public DateTimePeriod Appointed { get; set; }

		[DataMember]
		public DateTimePeriod Dismissed { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; } 
	}
}