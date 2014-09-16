using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ForeachArguments
	{
		public ForeachArguments()
		{
			ListParameter = new ArithmeticParameter();
			ItemParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter ListParameter { get; set; }

		[DataMember]
		public ArithmeticParameter ItemParameter { get; set; }
	}
}
