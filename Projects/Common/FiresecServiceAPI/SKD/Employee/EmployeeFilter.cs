using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class EmployeeFilter : OrganisationFilterBase
	{
		public EmployeeFilter()
			: base()
		{
			DepartmentUIDs = new List<Guid>();
			PositionUIDs = new List<Guid>();
			ScheduleUIDs = new List<Guid>();
			PersonType = PersonType.Employee;
		}

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		[DataMember]
		public List<Guid> DepartmentUIDs { get; set; }

		[DataMember]
		public List<Guid> PositionUIDs { get; set; }

		[DataMember]
		public List<Guid> ScheduleUIDs { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; }
	}
}