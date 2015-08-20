using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Employee : OrganisationElementBase, IOrganisationElement
	{
		public Employee()
			: base()
		{
			AdditionalColumns = new List<AdditionalColumn>();
		}

		[DataMember]
		public string FirstName { get; set; }

		[DataMember]
		public string SecondName { get; set; }

		[DataMember]
		public string LastName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public Guid PositionUID { get; set; }

		[DataMember]
		public string PositionName { get; set; }

		[DataMember]
		public bool IsPositionDeleted { get; set; }

		[DataMember]
		public Guid DepartmentUID { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }

		[DataMember]
		public bool IsDepartmentDeleted { get; set; }

		[DataMember]
		public Guid ScheduleUID { get; set; }

		[DataMember]
		public string ScheduleName { get; set; }
		
		[DataMember]
		public bool IsScheduleDeleted { get; set; }

		[DataMember]
		public DateTime ScheduleStartDate { get; set; }

		[DataMember]
		public Photo Photo { get; set; }

		[DataMember]
		public List<AdditionalColumn> AdditionalColumns { get; set; }

		[DataMember]
		public PersonType Type { get; set; }

		[DataMember]
		public string TabelNo;

		[DataMember]
		public DateTime CredentialsStartDate;

		[DataMember]
		public Guid? EscortUID;

		[DataMember]
		public string DocumentNumber { get; set; }

		[DataMember]
		public DateTime BirthDate { get; set; }

		[DataMember]
		public string BirthPlace { get; set; }

		[DataMember]
		public DateTime DocumentGivenDate { get; set; }

		[DataMember]
		public string DocumentGivenBy { get; set; }

		[DataMember]
		public DateTime DocumentValidTo { get; set; }

		[DataMember]
		public Gender Gender { get; set; }

		[DataMember]
		public string DocumentDepartmentCode { get; set; }

		[DataMember]
		public string Citizenship { get; set; }

		[DataMember]
		public EmployeeDocumentType DocumentType { get; set; }

		[DataMember]
		public DateTime LastEmployeeDayUpdate { get; set; }

		[DataMember]
		public string Phone { get; set; }

		public string Name { get { return LastName + " " + FirstName + " " + SecondName; } set { return; } }
		public string FIO { get { return LastName + " " + FirstName + (SecondName != null ? " " + SecondName : ""); } }
	}

	public enum Gender
	{
		[Description("Мужской")]
		Male,

		[Description("Женский")]
		Female
	}

	public enum EmployeeDocumentType
	{
		[Description("Паспорт РФ")]
		Passport,

		[Description("Паспорт иного государства")]
		ForeignPassport,
	}
}