using System;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeReplacement : OrganizationElementBase
	{
		public EmployeeReplacement()
			: base()
		{
			DateTimePeriod = new DateTimePeriod();
		}

		[DataMember]
		public DateTimePeriod DateTimePeriod { get; set; }

		[DataMember]
		public Guid? DepartmentUID { get; set; }

		[DataMember]
		public Guid? ScheduleUID { get; set; }

		[DataMember]
		public Guid? EmployeeUID { get; set; }
	}
}