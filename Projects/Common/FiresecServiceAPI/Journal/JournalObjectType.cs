using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalObjectType
	{
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

		[DescriptionAttribute("Устройство СКД")]
		SKDDevice,

		[DescriptionAttribute("Зона СКД")]
		SKDZone,

		[DescriptionAttribute("Видеоустройство")]
		VideoDevice,
	}
}