using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

namespace FiresecAPI.Automation
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
		//[Description("равно")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "IsEqual")]
		IsEqual,

		//[Description("не равно")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "IsNotEqual")]
        IsNotEqual,

		//[Description("больше")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "IsMore")]
        IsMore,

		//[Description("не больше")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "IsNotMore")]
        IsNotMore,

		//[Description("меньше")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "IsLess")]
        IsLess,

		//[Description("не меньше")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "IsNotLess")]
        IsNotLess,

		//[Description("Начинается с")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "StartsWith")]
        StartsWith,

		//[Description("Заканчивается на")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "EndsWith")]
        EndsWith,

		//[Description("Содержит")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.Condition), "Contains")]
        Contains
	}
}