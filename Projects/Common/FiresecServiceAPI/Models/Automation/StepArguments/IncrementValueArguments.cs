using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class IncrementValueArguments
	{
		public IncrementValueArguments()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }

		[DataMember]
		public IncrementType IncrementType { get; set; }
	}

	public enum IncrementType
	{
		//[Description("Инкремент")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.IncrementValueArguments), "Inc")]
        Inc,

		//[Description("Декремент")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.IncrementValueArguments), "Dec")]
		Dec,
	}
}