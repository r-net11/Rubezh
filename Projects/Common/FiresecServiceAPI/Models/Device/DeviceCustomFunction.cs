using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceCustomFunction
	{
		[DataMember]
		public int No { get; set; }

		[DataMember]
		public string Code { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Description { get; set; }
	}
}