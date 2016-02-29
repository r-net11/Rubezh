using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.SKD
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
		public bool IsEmptyDepartment { get; set; }

		[DataMember]
		public List<Guid> PositionUIDs { get; set; }

		[DataMember]
		public bool IsEmptyPosition { get; set; }

		[DataMember]
		public List<Guid> ScheduleUIDs { get; set; }

		[DataMember]
		public PersonType PersonType { get; set; }

		[DataMember]
		public bool IsAllPersonTypes { get; set; }

		public bool IsNotEmpty
		{
			get
			{
				return 
					(FirstName != null && FirstName != "") ||
					(LastName != null && LastName != "") ||
					(SecondName != null && SecondName != "") ||
					DepartmentUIDs.IsNotNullOrEmpty() ||
					PositionUIDs.IsNotNullOrEmpty() ||
					ScheduleUIDs.IsNotNullOrEmpty() ||
					OrganisationUIDs.IsNotNullOrEmpty() ||
					UIDs.IsNotNullOrEmpty();
			}
		}
	}

	public class HRFilter
	{
		public HRFilter()
		{
			EmployeeFilter = new EmployeeFilter();
			DepartmentFilter = new DepartmentFilter();
			PositionFilter = new PositionFilter();
			AdditionalColumnTypeFilter = new AdditionalColumnTypeFilter();
			AccessTemplateFilter = new AccessTemplateFilter();
			PassCardTemplateFilter = new PassCardTemplateFilter();
			CardFilter = new CardFilter();
		}
		public EmployeeFilter EmployeeFilter { get; set; }
		public DepartmentFilter DepartmentFilter { get; set; }
		public PositionFilter PositionFilter { get; set; }
		public AdditionalColumnTypeFilter AdditionalColumnTypeFilter { get; set; }
		public AccessTemplateFilter AccessTemplateFilter { get; set; }
		public PassCardTemplateFilter PassCardTemplateFilter { get; set; }
		public CardFilter CardFilter { get; set; }
	}
}