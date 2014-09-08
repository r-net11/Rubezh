using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class ArithmeticArguments
	{
		public ArithmeticArguments()
		{
			Variable1 = new ArithmeticParameter();
			Variable2 = new ArithmeticParameter();
			Result = new ArithmeticParameter();
		}

		[DataMember]
		public ValueType ValueType { get; set; }

		[DataMember]
		public ArithmeticOperationType ArithmeticOperationType { get; set; }

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public ArithmeticParameter Variable2 { get; set; }

		[DataMember]
		public ArithmeticParameter Result { get; set; }
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
