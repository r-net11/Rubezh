using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum TimeTrackPartType
	{
		[Description("Нет")]
		None,

		[Description("Пропуск")]
		PlanedOnly,

		[Description("Переработка не подтвержденная документом")]
		Overtime,

		[Description("Работа по графику")]
		AsPlanned,

		[Description("В рамках графика с уходом")]
		MissedButInsidePlan,

		[Description("Работа в перерыве")]
		InBrerak,

		[Description("Опоздание")]
		Late,

		[Description("Уход раньше")]
		EarlyLeave,

		[Description("Переработка по документу")]
		DocumentOvertime,

		[Description("Присутствие по документу")]
		DocumentPresence,

		[Description("Отсутствие по документу")]
		DocumentAbsence,
	}
}