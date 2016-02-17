using System.ComponentModel;

namespace FiresecAPI.Automation
{
	public enum EnumType
	{
		[DescriptionAttribute("Состояние")]
		StateType,

		[DescriptionAttribute("Тип устройства")]
		DriverType,

		[DescriptionAttribute("Права пользователя")]
		PermissionType,

		[DescriptionAttribute("Название события")]
		JournalEventNameType,

		[DescriptionAttribute("Уточнение")]
		JournalEventDescriptionType,

		[DescriptionAttribute("Тип объекта")]
		JournalObjectType,

		[DescriptionAttribute("Цвет")]
		ColorType,

		[DescriptionAttribute("Тип пропуска")]
		CardType
	}
}