using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI
{
	[DataContract]
	public class EmployeeDocument:SKDIsDeletedModel
	{
		[DataMember]
		public string Number { get; set; }

		[DataMember]
		public DateTime BirthDate { get; set; }

		[DataMember]
		public string BirthPlace { get; set; }

		[DataMember]
		public DateTime GivenDate { get; set; }

		[DataMember]
		public string GivenBy { get; set; }

		[DataMember]
		public DateTime ValidTo { get; set; }

		[DataMember]
		public Gender Gender { get; set; }

		[DataMember]
		public string DepartmentCode { get; set; }

		[DataMember]
		public string Citizenship { get; set; }

		[DataMember]
		public EmployeeDocumentType Type { get; set; }

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
