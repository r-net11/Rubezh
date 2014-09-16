using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class PauseArguments
	{
		public PauseArguments()
		{
			PauseParameter = new ArithmeticParameter();
			PauseParameter.VariableItem.IntValue = 1;
		}

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public ArithmeticParameter PauseParameter { get; set; }
	}
}
