using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI.Models
{
	[DataContract]
	public class DeviceParameterTemplate
	{
		[DataMember]
		public Device Device { get; set; }
	}
}
