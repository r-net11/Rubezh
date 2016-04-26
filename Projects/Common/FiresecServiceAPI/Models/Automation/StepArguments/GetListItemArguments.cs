using System.ComponentModel;
using System.Runtime.Serialization;
using Localization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class GetListItemArguments
	{
		public GetListItemArguments()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
			IndexArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }

		[DataMember]
		public Argument IndexArgument { get; set; }

		[DataMember]
		public PositionType PositionType { get; set; }
	}

	public enum PositionType
	{
		//[Description("Первый")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetListItemArguments), "First")]
        First,

		//[Description("Последний")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetListItemArguments), "Last")]
        Last,

		//[Description("По индексу")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.GetListItemArguments), "ByIndex")]
		ByIndex
	}
}