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

		[Description("Сотрудники")]
		Employee = 13,

		[Description("Должности")]
		Position = 14,

		[Description("Подразделения")]
		Department = 15,

		[Description("Пропуска")]
		Card = 16,

		[Description("Шаблоны пропусков")]
		PassCardTemplate = 17,

		[Description("Шаблоны доступа")]
		AccessTemplate = 18,

		[Description("Организации")]
		Organisation = 19,

		[Description("Дневные графики")]
		DayInterval = 20,

		[Description("Графики работы")]
		ScheduleScheme = 21,

		[Description("Графики работы сотрудников")]
		Schedule = 22,

		[Description("Прадничные дни")]
		Holiday = 23,

		[Description("Дополнительные колонки")]
		AdditionalColumn = 24,

		[Description("Дневной график ГК")]
		GKDayShedule = 25,

		[Description("График ГК")]
		GKShedule = 26,
	}
}