using System.Runtime.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDeviceParameterTemplate
	{
		[DataMember]
		public GKDevice GKDevice { get; set; }
	}
}