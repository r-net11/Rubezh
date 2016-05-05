using System.ComponentModel;
using LocalizationConveters;

namespace FiresecAPI.Automation
{
	public enum EnumType
	{
		//[DescriptionAttribute("Состояние")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "StateType")]
		StateType,

		//[DescriptionAttribute("Тип устройства")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "DriverType")]
        DriverType,

		//[DescriptionAttribute("Права пользователя")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "PermissionType")]
        PermissionType,

		//[DescriptionAttribute("Название события")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "JournalEventNameType")]
        JournalEventNameType,

		//[DescriptionAttribute("Уточнение")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "JournalEventDescriptionType")]
        JournalEventDescriptionType,

		//[DescriptionAttribute("Тип объекта")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "JournalObjectType")]
        JournalObjectType,

		//[DescriptionAttribute("Цвет")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "ColorType")]
        ColorType,

		//[DescriptionAttribute("Тип пропуска")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "CardType")]
        CardType,

		//[DescriptionAttribute("Режим доступа")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "AccessState")]
        AccessState,

		//[DescriptionAttribute("Статус двери")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "DoorStatus")]
        DoorStatus,

		//[DescriptionAttribute("Статус по взлому")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "BreakInStatus")]
        BreakInStatus,

		//[DescriptionAttribute("Статус соединения")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.EnumType), "ConnectionStatus")]
        ConnectionStatus
	}
}