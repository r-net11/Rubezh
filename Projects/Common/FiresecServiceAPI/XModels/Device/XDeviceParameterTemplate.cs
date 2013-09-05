using System.Runtime.Serialization;
using XFiresecAPI;

namespace FiresecAPI.XModels
{
	[DataContract]
	public class XDeviceParameterTemplate
	{
		[DataMember]
		public XDevice XDevice { get; set; }
	}
}
