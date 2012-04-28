using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeCardIndexFilter
	{
		[DataMember]
		public string ClockNumber { get; set; }
		[DataMember]
		public string LastName { get; set; }
		[DataMember]
		public string FirstName { get; set; }
		[DataMember]
		public string SecondName { get; set; }
		[DataMember]
		public int? DepartmentId { get; set; }
		[DataMember]
		public int? PositionId { get; set; }
		[DataMember]
		public int? GroupId { get; set; }
	}
}
