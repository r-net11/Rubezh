using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeHoliday : OrganizationElementBase
	{
		public EmployeeHoliday()
		{
			UID = Guid.NewGuid();
			EmployeeHolidayType = EmployeeHolidayType.Holiday;
		}

		[DataMember]
		public Guid UID { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public DateTime DateTime { get; set; }

		[DataMember]
		public EmployeeHolidayType EmployeeHolidayType { get; set; }

		[DataMember]
		public DateTime ShortageTime { get; set; }

		[DataMember]
		public DateTime TransitionDateTime { get; set; }

	}
}