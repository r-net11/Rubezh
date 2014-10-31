using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace FiresecAPI.GK
{
	/// <summary>
	/// Тип периодичности графика
	/// </summary>
	public enum GKSchedulePeriodType
	{
		[Description("Недельный")]
		Weekly,

		[Description("Суточный")]
		Dayly,

		[Description("Произвольный")]
		Custom,

		[Description("Непериодичный")]
		NonPeriodic
	}
}