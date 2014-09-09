using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class PauseArguments
	{
		public PauseArguments()
		{
			Pause = new ArithmeticParameter();
			Pause.VariableItem.IntValue = 1;
		}

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public ArithmeticParameter Pause { get; set; }
	}
}
