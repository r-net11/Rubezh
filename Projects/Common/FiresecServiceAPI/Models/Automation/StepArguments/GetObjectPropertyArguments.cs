using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GetObjectPropertyArguments
	{
		public GetObjectPropertyArguments()
		{
			ObjectParameter = new ArithmeticParameter();
			ResultParameter = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter ObjectParameter { get; set; }

		[DataMember]
		public ArithmeticParameter ResultParameter { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}
}
