using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class FindObjectCondition
	{
		public FindObjectCondition()
		{
			SourceArgument = new Argument();
		}

		[DataMember]
		public ConditionType ConditionType { get; set; }

		[DataMember]
		public Argument SourceArgument { get; set; }

		[DataMember]
		public Property Property { get; set; }
	}

	public enum Property
	{
		[Description("Примечание")]
		Description,

		[Description("Имя")]
		Name,

		[Description("Адрес")]
		IntAddress,

		[Description("АЛС")]
		ShleifNo,

		[Description("Состояние")]
		State,

		[Description("Номер")]
		No,

		[Description("Тип")]
		Type,

		[Description("Идентификатор")]
		Uid,

		[Description("Задержка")]
		Delay,

		[Description("Текущая задержка")]
		CurrentDelay,

		[Description("Удержание")]
		Hold,

		[Description("Текущее удержание")]
		CurrentHold,

		[Description("Режим после удержания")]
		DelayRegime
	}
}