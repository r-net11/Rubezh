using System;
using System.Runtime.Serialization;
using FiresecAPI.GK;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlGKDeviceArguments
	{
		public ControlGKDeviceArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid DeviceUid { get; set; }

		[DataMember]
		public XStateBit Command { get; set; }
	}
}
