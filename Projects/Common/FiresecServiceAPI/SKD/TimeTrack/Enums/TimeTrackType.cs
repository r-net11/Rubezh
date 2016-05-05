using System.ComponentModel;

namespace StrazhAPI.SKD
{
	public enum TimeTrackType
	{
		[Description("Нет данных")]
		None,

		[Description("Баланс")]
		Balance,

		[Description("Явка")]
		Presence,

		[Description("Отсутствие")]
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

		[Description("Праздничный день")]
		Holiday,

		[Description("Переработка по документу")]
		DocumentOvertime,

		[Description("Присутствие по документу")]
		DocumentPresence,

		[Description("Отсутствие по неуважительной причине")]
		DocumentAbsence,

		[Description("Отсутствие по уважительной причине")]
		DocumentAbsenceReasonable,
	}
}