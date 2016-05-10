using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ArithmeticArguments
	{
		public ArithmeticArguments()
		{
			Argument1 = new Argument();
			Argument2 = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ArithmeticOperationType ArithmeticOperationType { get; set; }

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public Argument Argument1 { get; set; }

		[DataMember]
		public Argument Argument2 { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }
	}

	public enum ArithmeticOperationType
	{
		//[Description("Сложение")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Add")]
		Add,

		//[Description("Вычитание")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Sub")]
        Sub,

		//[Description("Умножение")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Multi")]
        Multi,

		//[Description("Деление")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Div")]
        Div,

		//[Description("И")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "And")]
        And,

		//[Description("Или")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Or")]
        Or
	}

	public enum TimeType
	{
		//[Description("секунд")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Sec")]
        Sec,

		//[Description("минут")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Min")]
        Min,

		//[Description("часов")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Hour")]
        Hour,

		//[Description("дней")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ArithmeticArguments), "Day")]
        Day
	}
}