using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI
{
	[DataContract]
	public class Employee : OrganizationElementBase
	{
		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public DateTime Appointed { get; set; }

		[DataMember]
		public DateTime Dismissed { get; set; }

		[DataMember]
		public Guid? PositionUID { get; set; }

		[DataMember]
		public Guid? DepartmentUID { get; set; }

		[DataMember]
		public Guid? ReplacementUID { get; set; }

		[DataMember]
		public Guid? ScheduleUID { get; set; }

		[DataMember]
		public Guid? PhotoUID { get; set; }

		[DataMember]
		public List<Guid> AdditionalColumnUIDs { get; set; }

		[DataMember]
		public List<Guid> CardUIDs { get; set; }

		[DataMember]
		public PersonType Type { get; set; }
	}

	public enum PersonType
	{
		[DescriptionAttribute("Работник")]
		Employee,
		[DescriptionAttribute("Гость")]
		Guest
	}
}