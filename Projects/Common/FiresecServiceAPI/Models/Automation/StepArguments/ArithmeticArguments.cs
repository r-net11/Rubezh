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
		public ValueType ArithmeticValueType { get; set; }

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

	public class ArithmeticParameter
	{
		public ArithmeticParameter()
		{
			VariableUid = new Guid();
			BoolValue = false;
			DateTimeValue = DateTime.Now;
			IntValue = 0;
			StringValue = "";
			TypeValue = "";
		}

		[DataMember]
		public VariableType VariableType { get; set; }

		[DataMember]
		public bool BoolValue { get; set; }

		[DataMember]
		public DateTime DateTimeValue { get; set; }

		[DataMember]
		public int IntValue { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid UidValue { get; set; }

		[DataMember]
		public string TypeValue { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }
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
		Or,

		[Description("Конкатенация")]
		Concat
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
