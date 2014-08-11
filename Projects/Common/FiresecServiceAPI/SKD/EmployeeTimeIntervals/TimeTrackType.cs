using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
		OutShedule,

		[Description("Выходной")]
		DayOff,

		[Description("Праздник")]
		Holiday,

		[Description("Больничный")]
		Ill,

		[Description("Отпуск")]
		Vacation,

		[Description("Командировка")]
		Trip
	}
}