using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalObjectType
	{
		[DescriptionAttribute("Нет")]
		None,

		[DescriptionAttribute("Устройства ГК")]
		GKDevice,

		[DescriptionAttribute("Направления ГК")]
		GKDirection,

		[DescriptionAttribute("МПТ ГК")]
		GKMPT,

		[DescriptionAttribute("Насосные станции ГК")]
		GKPumpStation,

		[DescriptionAttribute("Задержки ГК")]
		GKDelay,

		[DescriptionAttribute("ПИМ ГК")]
		GKPim,

		[DescriptionAttribute("Точки доступа ГК")]
		GKDoor,

		[DescriptionAttribute("Устройства Страж")]
		SKDDevice,

		[DescriptionAttribute("Зоны Страж")]
		SKDZone,

		[DescriptionAttribute("Точки доступа Страж")]
		SKDDoor,

		[DescriptionAttribute("Видеоустройства")]
		VideoDevice,

		[DescriptionAttribute("Пользователи ГК")]
		GKUser,

		[DescriptionAttribute("Зоны СКД ГК")]
		GKSKDZone,
	}
}