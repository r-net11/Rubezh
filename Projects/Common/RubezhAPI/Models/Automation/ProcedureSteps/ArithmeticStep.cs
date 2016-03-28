using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ArithmeticStep : ProcedureStep
	{
		public ArithmeticStep()
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

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Arithmetics; } }
	}

	public enum ArithmeticOperationType
	{
		[Description("Сложение")]
		Add,

		[Description("Вычитание")]
		Sub,

		[Description("Умножение")]
		Multi,

		[Description("Деление")]
		Div,

		[Description("И")]
		And,

		[Description("Или")]
		Or
	}
}