using System.ComponentModel;

namespace FiresecAPI.Journal
{
	public enum JournalObjectType
	{
		[DescriptionAttribute("Нет")]
		None = 0,

		[DescriptionAttribute("Устройства")]
		GKDevice = 1,

		[DescriptionAttribute("Пожарные зоны")]
		GKZone = 2,

		[DescriptionAttribute("Направления")]
		GKDirection = 3,

		[DescriptionAttribute("МПТ")]
		GKMPT = 4,

		[DescriptionAttribute("Насосные станции")]
		GKPumpStation = 5,

		[DescriptionAttribute("Задержки")]
		GKDelay = 6,

		[DescriptionAttribute("ПИМ")]
		GKPim = 7,

		[DescriptionAttribute("Охранные зоны")]
		GKGuardZone = 8,

		[DescriptionAttribute("Точки доступа")]
		GKDoor = 9,

		[DescriptionAttribute("Видеоустройства")]
		VideoDevice = 10,

		[DescriptionAttribute("Пользователи прибора")]
		GKUser = 11,

		[DescriptionAttribute("Зоны СКД")]
		GKSKDZone = 12,
	}
}