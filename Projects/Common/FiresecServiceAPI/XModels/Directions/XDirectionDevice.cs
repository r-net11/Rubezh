using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class XDirectionDevice
	{
		public XDevice Device { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public XStateBit StateBit { get; set; }
	}
}