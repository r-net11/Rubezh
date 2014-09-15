﻿using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class FindObjectCondition
	{
		public FindObjectCondition()
		{
			Variable2 = new ArithmeticParameter();
		}

		[DataMember]
		public ConditionType ConditionType { get; set; }

		[DataMember]
		public ArithmeticParameter Variable2 { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}

	public enum Property
	{
		[Description("Примечание")]
		Description,

		[Description("Адрес")]
		IntAddress,

		[Description("Шлейф")]
		ShleifNo,

		[Description("Состояние")]
		State,

		[Description("Номер")]
		No,

		[Description("Тип")]
		Type,

		[Description("Задержка")]
		Delay,
		
		[Description("Удержание")]
		Hold,

		[Description("Режим")]
		DelayRegime
	}
}
