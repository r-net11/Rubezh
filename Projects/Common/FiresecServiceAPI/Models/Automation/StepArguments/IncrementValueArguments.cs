using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class IncrementValueArguments
	{
		public IncrementValueArguments()
		{
			Uid = Guid.NewGuid();
			Variable1 = new ArithmeticParameter();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public IncrementType IncrementType { get; set; }
	}

	public enum IncrementType
	{
		[Description("Инкремент")]
		Inc,

		[Description("Декремент")]
		Dec,
	}
}
