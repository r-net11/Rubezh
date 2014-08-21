using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class PauseArguments
	{
		public PauseArguments()
		{
			Uid = Guid.NewGuid();
			Variable = new ArithmeticParameter();
			Variable.IntValue = 1;
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ArithmeticParameter Variable { get; set; }
	}
}
