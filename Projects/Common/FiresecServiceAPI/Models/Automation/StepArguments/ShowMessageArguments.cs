using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ShowMessageArguments
	{
		public ShowMessageArguments()
		{
			Variable1 = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }
	}
}