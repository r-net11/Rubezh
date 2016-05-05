using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace FiresecAPI.Automation
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
		//[Description("Примечание")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "Description")]
		Description,

		//[Description("Имя")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "Name")]
        Name,

		//[Description("Адрес")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "IntAddress")]
        IntAddress,

		//[Description("Шлейф")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "ShleifNo")]
        ShleifNo,

		//[Description("Состояние")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "State")]
        State,

		//[Description("Номер")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "No")]
        No,

		//[Description("Тип")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "Type")]
        Type,

		//[Description("Идентификатор")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "Uid")]
        Uid,

		//[Description("Удержание")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "Hold")]
        Hold,

		//[Description("Текущее удержание")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "CurrentHold")]
        CurrentHold,

		//[Description("Режим")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "DelayRegime")]
        DelayRegime,

		//[DescriptionAttribute("Режим доступа")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "AccessState")]
        AccessState,

		//[DescriptionAttribute("Статус двери")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "DoorStatus")]
        DoorStatus,

		//[DescriptionAttribute("Статус по взлому")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "BreakInStatus")]
        BreakInStatus,

		//[DescriptionAttribute("Статус соединения")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.FindObjectCondition), "ConnectionStatus")]
        ConnectionStatus
	}
}