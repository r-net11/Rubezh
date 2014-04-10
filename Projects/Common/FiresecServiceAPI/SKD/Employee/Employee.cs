using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

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
		public Position Position { get; set; }

		[DataMember]
		public Guid? DepartmentUID { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }

		[DataMember]
		public List<Guid> ReplacementUIDs { get; set; }

		[DataMember]
		public Guid? ScheduleUID { get; set; }

		[DataMember]
		public Photo Photo { get; set; }

		[DataMember]
		public List<AdditionalColumn> AdditionalColumns { get; set; }

		[DataMember]
		public List<SKDCard> Cards { get; set; }

		[DataMember]
		public PersonType Type { get; set; }

		[DataMember]
		public EmployeeReplacement CurrentReplacement { get; set; }

		public bool IsReplaced
		{
			get { return CurrentReplacement != null; }
		}
	}
}