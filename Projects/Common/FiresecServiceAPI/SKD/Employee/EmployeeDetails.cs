using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	public class EmployeeDetails:OrganizationElementBase
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
		public Department Department { get; set; }

		[DataMember]
		public List<EmployeeReplacement> Replacements { get; set; }

		[DataMember]
		public Schedule Schedule { get; set; }

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

		public Employee GetEmployee()
		{
			return new Employee
			{
				UID = UID,
				AdditionalColumnUIDs = SKDHelper.GetUIDs(AdditionalColumns),
				Appointed = Appointed,
				Dismissed = Dismissed,
				CardUIDs = SKDHelper.GetUIDs(Cards),
				CurrentReplacement = CurrentReplacement,
				DepartmentUID = SKDHelper.GetUID(Department),
				FirstName = FirstName,
				IsDeleted = IsDeleted,
				LastName = LastName,
				OrganizationUID = OrganizationUID,
				PhotoUID = SKDHelper.GetUID(Photo),
				PositionUID = SKDHelper.GetUID(Position),
				RemovalDate = RemovalDate,
				ReplacementUIDs = SKDHelper.GetUIDs(Replacements),
				ScheduleUID = SKDHelper.GetUID(Schedule),
				SecondName = SecondName,
				Type = Type
			};
		}
	}
}
