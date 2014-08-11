using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class FindObjectCondition
	{
		public FindObjectCondition()
		{
			Uid = new Guid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public ConditionType ConditionType { get; set; }

		[DataMember]
		public StringConditionType StringConditionType { get; set; }

		[DataMember]
		public int IntValue{ get; set; }

		[DataMember]
		public string StringValue { get; set; }

		[DataMember]
		public StateType StateTypeValue { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}

	public enum Property
	{
		[Description("Имя")]
		Name,

		[Description("Адрес")]
		IntAddress,

		[Description("Шлейф")]
		ShleifNo,

		[Description("Состояние")]
		DeviceState,

		[Description("Номер")]
		No,

		[Description("Тип")]
		ZoneType,

		[Description("Задержка")]
		Delay,
		
		[Description("Удержание")]
		Hold,

		[Description("Режим")]
		DelayRegime
	}

	public enum PropertyType
	{
		[DescriptionAttribute("Целое")]
		Integer,

		[DescriptionAttribute("Логическое")]
		Boolean,

		[DescriptionAttribute("Дата и время")]
		DateTime,

		[DescriptionAttribute("Строка")]
		String,

		[Description("Перечисление")]
		Enum
	}
}
