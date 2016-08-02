using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class Condition
	{
		public Condition()
		{
			Uid = new Guid();
			Argument1 = new Argument();
			Argument2 = new Argument();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Argument Argument1 { get; set; }

		[DataMember]
		public Argument Argument2 { get; set; }

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