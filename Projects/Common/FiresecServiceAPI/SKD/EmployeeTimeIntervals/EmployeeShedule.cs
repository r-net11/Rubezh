using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeShedule : OrganizationElementBase
	{
		public EmployeeShedule()
		{
			UID = Guid.NewGuid();
			EmployeeSheduleType = EmployeeSheduleType.Week;
			EmployeeSheduleParts = new List<EmployeeShedulePart>();
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsIgnoreHoliday { get; set; }

		[DataMember]
		public bool IsOnlyFirstEnter { get; set; }

		[DataMember]
		public EmployeeSheduleType EmployeeSheduleType { get; set; }

		[DataMember]
		public Guid SheduleUID { get; set; }

		[DataMember]
		public bool IsDefault { get; set; }

		[DataMember]
		public List<EmployeeShedulePart> EmployeeSheduleParts { get; set; }
	}
}