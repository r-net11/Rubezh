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
		public DevicePropertyType DevicePropertyType { get; set; }

		[DataMember]
		public ZonePropertyType ZonePropertyType { get; set; }

		[DataMember]
		public DirectionPropertyType DirectionPropertyType { get; set; }

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
		public PropertyType PropertyType { get; set; }

	}

	public enum DevicePropertyType
	{
		[Description("Имя")]
		Name,

		[Description("Адрес")]
		IntAddress,

		[Description("Шлейф")]
		ShleifNo,

		[Description("Состояние")]
		DeviceState
	}

	public enum ZonePropertyType
	{
		[Description("Имя")]
		Name,

		[Description("Номер")]
		No,

		[Description("Тип")]
		ZoneType
	}

	public enum DirectionPropertyType
	{
		[Description("Имя")]
		Name,

		[Description("Номер")]
		No,

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

		[DescriptionAttribute("Перечисление")]
		Enum
	}
}
