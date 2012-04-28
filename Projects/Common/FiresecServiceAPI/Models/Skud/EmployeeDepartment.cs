using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeDepartment
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string Value { get; set; }
		[DataMember]
		public int? ParentId { get; set; }
	}
}
