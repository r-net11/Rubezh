using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalObjectType
	{
		[DescriptionAttribute("Нет")]
		None = 0,

		[DescriptionAttribute("Устройства ГК")]
		GKDevice = 1,

		[DescriptionAttribute("Зоны ГК")]
		GKZone = 2,

		[DescriptionAttribute("Направления ГК")]
		GKDirection = 3,

		[DescriptionAttribute("МПТ ГК")]
		GKMPT = 4,

		[DescriptionAttribute("Насосные станции ГК")]
		GKPumpStation = 5,

		[DescriptionAttribute("Задержки ГК")]
		GKDelay = 6,

		[DescriptionAttribute("ПИМ ГК")]
		GKPim = 7,

		[DescriptionAttribute("Охранные Зоны ГК")]
		GKGuardZone = 8,

		[DescriptionAttribute("Точки доступа ГК")]
		GKDoor = 9,

		[DescriptionAttribute("Видеоустройства")]
		VideoDevice = 10,

		[DescriptionAttribute("Пользователи ГК")]
		GKUser = 11,

		[DescriptionAttribute("Зоны СКД ГК")]
		GKSKDZone = 12,
	}
}