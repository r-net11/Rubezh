using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalObjectType
	{
		[DescriptionAttribute("Нет")]
		None,

		[DescriptionAttribute("Устройства ГК")]
		GKDevice,

		[DescriptionAttribute("Зоны ГК")]
		GKZone,

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

		[DescriptionAttribute("Охранные Зоны ГК")]
		GKGuardZone,

		[DescriptionAttribute("Устройства СКД")]
		SKDDevice,

		[DescriptionAttribute("Зоны СКД")]
		SKDZone,

		[DescriptionAttribute("Точки доступа СКД")]
		SKDDoor,

		[DescriptionAttribute("Видеоустройства")]
		VideoDevice,
	}
}