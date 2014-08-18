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
		public OperationType OperationType { get; set; }

		[DataMember]
		public ArithmeticType ArithmeticType { get; set; }

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
		}

		[DataMember]
		public VariableType VariableType { get; set; }

		[DataMember]
		public int Value { get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }
	}


	public enum OperationType
	{
		[Description("Операции с целыми")]
		IntegerOperation,

		[Description("Операции с логическими")]
		BoolOperation,

		[Description("Операции с временными")]
		DateTimeOperation,

		[Description("Операции со строковыми")]
		StringOperation
	}

	public enum ArithmeticType
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
}
