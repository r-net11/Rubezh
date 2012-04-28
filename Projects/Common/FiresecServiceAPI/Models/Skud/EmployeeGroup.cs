using System.Runtime.Serialization;

namespace FiresecAPI.Models.Skud
{
	[DataContract]
	public class EmployeeGroup
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string Value { get; set; }
	}
}
