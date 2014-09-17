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
			GKDeviceParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter GKDeviceParameter { get; set; }

		[DataMember]
		public XStateBit Command { get; set; }
	}
}
