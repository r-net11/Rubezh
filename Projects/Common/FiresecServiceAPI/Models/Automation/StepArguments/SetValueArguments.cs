using System;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class SetValueArguments
	{
		public SetValueArguments()
		{
			Uid = Guid.NewGuid();
			Variable1 = new ArithmeticParameter();
			Result = new ArithmeticParameter();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ArithmeticParameter Result { get; set; }

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

	}
}
