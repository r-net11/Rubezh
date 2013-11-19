using System.Runtime.Serialization;
using XFiresecAPI;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceParameterTemplate
	{
		[DataMember]
		public XDevice XDevice { get; set; }
	}
}