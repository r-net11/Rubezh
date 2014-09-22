using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class PauseArguments
	{
		public PauseArguments()
		{
			PauseParameter = new Variable();
			PauseParameter.ExplicitValue.IntValue = 1;
		}

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public Variable PauseParameter { get; set; }
	}
}
