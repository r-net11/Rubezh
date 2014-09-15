using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GetObjectPropertyArguments
	{
		public GetObjectPropertyArguments()
		{
			Variable1 = new ArithmeticParameter();
			Result = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public ArithmeticParameter Result { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}
}
