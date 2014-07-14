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

		[DescriptionAttribute("Охранная Зона ГК")]
		GKGuardZone,

		[DescriptionAttribute("Устройство СКД")]
		SKDDevice,

		[DescriptionAttribute("Зона СКД")]
		SKDZone,

		[DescriptionAttribute("Точка доступа СКД")]
		SKDDoor,

		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,
	}
}