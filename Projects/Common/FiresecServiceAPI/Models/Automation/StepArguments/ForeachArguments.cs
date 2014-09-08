using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ForeachArguments
	{
		public ForeachArguments()
		{
			ListVariable = new ArithmeticParameter();
			ItemVariable = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter ListVariable { get; set; }

		[DataMember]
		public ArithmeticParameter ItemVariable { get; set; }
	}
}
