using System;
using System.Runtime.Serialization;

namespace XFiresecAPI
{
	[DataContract]
	public class DirectionDevice
	{
		public XDevice Device { get; set; }

		[DataMember]
		public Guid DeviceUID { get; set; }

		[DataMember]
		public XStateType StateType { get; set; }
	}
}