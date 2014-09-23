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
			Parameter1 = new Variable();
			Parameter2 = new Variable();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Variable Parameter1 { get; set; }

		[DataMember]
		public Variable Parameter2 { get; set; }

		[DataMember]
		public ConditionType ConditionType { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }
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

		[Description("Начинается с")]
		StartsWith,

		[Description("Заканчивается на")]
		EndsWith,

		[Description("Содержит")]
		Contains
	}
}
