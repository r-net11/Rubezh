using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class Employee : OrganisationElementBase
	{
		public Employee() : base()
		{
			Cards = new List<SKDCard>();
			ReplacementUIDs = new List<Guid>();
			AdditionalColumns = new List<AdditionalColumn>();
		}

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
		public ShortPosition Position { get; set; }

		[DataMember]
		public Guid? DepartmentUID { get; set; }

		[DataMember]
		public string DepartmentName { get; set; }

		[DataMember]
		public List<Guid> ReplacementUIDs { get; set; }

		[DataMember]
		public Guid? ScheduleUID { get; set; }

		[DataMember]
		public DateTime ScheduleStartDate { get; set; }

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

		[DataMember]
		public int TabelNo;

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

		public bool IsReplaced
		{
			get { return CurrentReplacement != null; }
		}
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
		[Description("Пасспорт РФ")]
		Passport,
		[Description("Пасспорт иного государства")]
		ForeignPassport,
	}
}