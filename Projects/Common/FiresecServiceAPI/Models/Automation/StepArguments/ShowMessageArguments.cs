using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ShowMessageArguments
	{
		public ShowMessageArguments()
		{
			MessageParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter MessageParameter { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }
	}
}