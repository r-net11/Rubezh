using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class XDeviceParameterTemplate
	{
		[DataMember]
		public XDevice XDevice { get; set; }
	}
}