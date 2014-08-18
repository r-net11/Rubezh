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
			GlobalVariableUid = new Guid();
			VariableUid = new Guid();
		}

		[DataMember]
		public VariableType VariableType { get; set; }

		[DataMember]
		public int Value { get; set; }

		[DataMember]
		public Guid GlobalVariableUid { get; set; }

		[DataMember]
		public Guid VariableUid { get; set; }
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
		Div
	}
}
