using System.ComponentModel;

namespace FiresecAPI.SKD
{
	public enum TimeTrackType
	{
		[Description("Нет данных")]
		None,

		[Description("Прогул")]
		Missed,

		[Description("По графику")]
		AsPlanned,

		[Description("Опоздание")]
		Late,

		[Description("Уход раньше")]
		EarlyLeave,

		[Description("Сверхурочно")]
		Overtime,

		[Description("Выходной")]
		DayOff,

		[Description("Праздник")]
		Holiday,

		[Description("Документ")]
		Document,
	}
}