using System.Runtime.Serialization;

namespace FiresecAPI.Models.SKDDatabase
{
	[DataContract]
	public class EmployeePosition
	{
		[DataMember]
		public int Id { get; set; }
		[DataMember]
		public string Value { get; set; }
	}
}
