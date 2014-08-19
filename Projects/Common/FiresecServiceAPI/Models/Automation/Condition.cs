using System;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class Condition
	{
		public Condition()
		{
			Uid = new Guid();
			Variable1 = new ArithmeticParameter();
			Variable2 = new ArithmeticParameter();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public ArithmeticParameter Variable2 { get; set; }

		[DataMember]
		public ConditionType ConditionType { get; set; }

		[DataMember]
		public ValueType ConditionValueType { get; set; }
	}

	public enum ConditionType
	{
		[Description("равно")]
		IsEqual,

		[Description("не равно")]
		IsNotEqual,

		[Description("больше")]
		IsMore,

		[Description("не больше")]
		IsNotMore,

		[Description("меньше")]
		IsLess,

		[Description("не меньше")]
		IsNotLess,
	}
}
