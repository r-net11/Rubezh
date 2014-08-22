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

		[Description("Отсутствие в рамках графика")]
		AbsenceInsidePlan,

		[Description("Явка в перерыве")]
		PresenceInBrerak,

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

		[Description("Переработка по документу")]
		DocumentOvertime,

		[Description("Присутствие по документу")]
		DocumentPresence,

		[Description("Отсутствие по документу")]
		DocumentAbsence,
	}
}