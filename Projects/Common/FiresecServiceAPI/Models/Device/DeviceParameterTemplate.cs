using System.Runtime.Serialization;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceParameterTemplate
	{
		[DataMember]
		public Device Device { get; set; }
	}
}
