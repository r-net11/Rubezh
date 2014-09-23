using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ArithmeticArguments
	{
		public ArithmeticArguments()
		{
			Parameter1 = new Variable();
			Parameter2 = new Variable();
			ResultParameter = new Variable();
		}

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public ArithmeticOperationType ArithmeticOperationType { get; set; }

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public Variable Parameter1 { get; set; }

		[DataMember]
		public Variable Parameter2 { get; set; }

		[DataMember]
		public Variable ResultParameter { get; set; }
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

	public enum TimeType
	{
		[Description("секунд")]
		Sec,

		[Description("минут")]
		Min,

		[Description("часов")]
		Hour,

		[Description("дней")]
		Day
	}
}
