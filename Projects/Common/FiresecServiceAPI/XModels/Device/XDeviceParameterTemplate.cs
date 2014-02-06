using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDeviceParameterTemplate
	{
		[DataMember]
		public XDevice XDevice { get; set; }
	}
}