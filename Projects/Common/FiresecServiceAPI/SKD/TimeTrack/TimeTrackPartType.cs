using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum TimeTrackPartType
	{
		[Description("Нет")]
		None,

		[Description("Пропуск")]
		PlanedOnly,

		[Description("Работа вне графика")]
		RealOnly,

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
	}
}