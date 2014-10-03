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
			GKDeviceParameter = new Argument();
		}

		[DataMember]
		public Argument GKDeviceParameter { get; set; }

		[DataMember]
		public GKStateBit Command { get; set; }
	}
}