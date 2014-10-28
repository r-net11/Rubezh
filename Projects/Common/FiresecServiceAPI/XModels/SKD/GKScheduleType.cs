using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип графика
	/// </summary>
	public enum GKScheduleType
	{
		[Description("Недельный")]
		Weekly,

		[Description("Суточный")]
		Dayly,

		[Description("Произвольный")]
		Custom,

		[Description("непериодичный")]
		NonPeriodic
	}
}