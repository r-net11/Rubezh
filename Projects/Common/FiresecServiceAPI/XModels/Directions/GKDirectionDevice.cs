using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace FiresecAPI.GK
{
	[DataContract]
	public class GKDirectionDevice
	{
		[XmlIgnore]
		public GKDevice Device { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public GKStateBit StateBit { get; set; }
	}
}