using System.ComponentModel;

namespace RubezhAPI.Journal
{
	public enum JournalObjectType
	{
		[Description("Нет")]
		None = 0,

		[Description("Устройства")]
		GKDevice = 1,

		[Description("Пожарные зоны")]
		GKZone = 2,

		[Description("Направления")]
		GKDirection = 3,

		[Description("МПТ")]
		GKMPT = 4,

		[Description("Насосные станции")]
		GKPumpStation = 5,

		[Description("Задержки")]
		GKDelay = 6,

		[Description("ПИМ")]
		GKPim = 7,

		[Description("Охранные зоны")]
		GKGuardZone = 8,

		[Description("Точки доступа")]
		GKDoor = 9,

		[Description("Канал Rvi")]
		Camera = 10,

		[Description("Пользователи прибора")]
		GKUser = 11,

		[Description("Зоны СКД")]
		GKSKDZone = 12,

		[Description("Устройство Rvi")]
		RviDevice = 13
	}
}