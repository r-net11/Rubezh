using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract, Serializable]
	public class PauseArguments
	{
		public PauseArguments()
		{
			PauseArgument = new Argument();
			PauseArgument.ExplicitValue.IntValue = 1;
		}

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public Argument PauseArgument { get; set; }
	}
}