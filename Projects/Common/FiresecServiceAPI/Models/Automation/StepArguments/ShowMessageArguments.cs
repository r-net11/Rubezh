using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class ShowMessageArguments
	{
		public ShowMessageArguments()
		{
			Variable1 = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public ValueType ValueType { get; set; }
	}
}