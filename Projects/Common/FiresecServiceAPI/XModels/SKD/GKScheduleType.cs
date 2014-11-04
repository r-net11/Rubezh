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
		[Description("График доступа")]
		Access,

		[Description("Праздничные дни")]
		Holiday,

		[Description("Рабочие выходные")]
		WorkHoliday,
	}
}