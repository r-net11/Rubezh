using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeCardDetails
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string ClockNumber { get; set; }
		[DataMember]
		public string Comment { get; set; }
		[DataMember]
		public int? DepartmentId { get; set; }
		[DataMember]
		public string Email { get; set; }
		[DataMember]
		public int? GroupId { get; set; }
		[DataMember]
		public string Phone { get; set; }
		[DataMember]
		public int? PositionId { get; set; }
		[DataMember]
		public string Address { get; set; }
		[DataMember]
		public string AddressFact { get; set; }
		[DataMember]
		public string BirthPlace { get; set; }
		[DataMember]
		public DateTime? Birthday { get; set; }
		[DataMember]
		public string Cell { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string ITN { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string PassportCode { get; set; }
		[DataMember]
		public DateTime? PassportDate { get; set; }
		[DataMember]
		public string PassportEmitter { get; set; }
		[DataMember]
		public string PassportNumber { get; set; }
		[DataMember]
		public string PassportSerial { get; set; }
		[DataMember]
		public byte[] Photo { get; set; }
		[DataMember]
		public string SNILS { get; set; }
		[DataMember]
		public string SecondName { get; set; }
		[DataMember]
		public int? SexId { get; set; }
	}
}
