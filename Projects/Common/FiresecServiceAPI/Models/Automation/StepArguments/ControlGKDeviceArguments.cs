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
			GKDeviceArgument = new Argument();
		}

		[DataMember]
		public Argument GKDeviceArgument { get; set; }

		[DataMember]
		public GKStateBit Command { get; set; }
	}
}