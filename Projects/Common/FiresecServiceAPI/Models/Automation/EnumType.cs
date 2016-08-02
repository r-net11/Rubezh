using System.ComponentModel;

namespace StrazhAPI.Automation
{
	public enum EnumType
	{
		[Description("Состояние")]
		StateType,

		[Description("Тип устройства")]
		DriverType,

		[Description("Права пользователя")]
		PermissionType,

		[Description("Название события")]
		JournalEventNameType,

		[Description("Уточнение")]
		JournalEventDescriptionType,

		[Description("Тип объекта")]
		JournalObjectType,

		[Description("Цвет")]
		ColorType,

		[Description("Тип пропуска")]
		CardType,

		[Description("Режим доступа")]
		AccessState,

		[Description("Статус двери")]
		DoorStatus,

		[Description("Статус по взлому")]
		BreakInStatus,

		[Description("Статус соединения")]
		ConnectionStatus
	}
}