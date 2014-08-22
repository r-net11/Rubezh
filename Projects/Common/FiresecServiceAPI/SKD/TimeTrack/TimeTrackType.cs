using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum TimeTrackType
	{
		[Description("Нет данных")]
		None,

		[Description("Явка")]
		Presence,

		[Description("Прогул")]
		Absence,

		[Description("Опоздание")]
		Late,

		[Description("Уход раньше")]
		EarlyLeave,

		[Description("Сверхурочно")]
		Overtime,

		[Description("Работа ночью")]
		Night,

		[Description("Выходной")]
		DayOff,

		[Description("Праздник")]
		Holiday,

		[Description("Документ")]
		Document,
	}
}