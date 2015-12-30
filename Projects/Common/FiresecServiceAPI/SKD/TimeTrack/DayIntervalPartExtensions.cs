using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD
{
	public static class DayIntervalPartExtensions
	{
		/// <summary>
		/// Проверяет, что временной интервал дневного графика имеет длину равную нулю
		/// </summary>
		/// <param name="dayIntervalPart">Временной интервал дневного графика</param>
		/// <returns>true - длина временного интервала равна 0, false - длина временного интервала отлична от 0</returns>
		public static bool IsZeroLength(this DayIntervalPart dayIntervalPart)
		{
			return dayIntervalPart.EndTime == dayIntervalPart.BeginTime;
		}

		public static bool HasIntersectionWith(this DayIntervalPart dayIntervalPart, DayIntervalPart otherDayIntervalPart)
		{
			var beginTime = dayIntervalPart.BeginTime;
			var endTime = dayIntervalPart.TransitionType == DayIntervalPartTransitionType.Night
				? dayIntervalPart.EndTime.Add(TimeSpan.FromDays(1))
				: dayIntervalPart.EndTime;
			var otherBeginTime = otherDayIntervalPart.BeginTime;
			var otherEndTime = otherDayIntervalPart.TransitionType == DayIntervalPartTransitionType.Night
				? otherDayIntervalPart.EndTime.Add(TimeSpan.FromDays(1))
				: otherDayIntervalPart.EndTime;

			if (endTime < otherBeginTime || beginTime > otherEndTime)
				return false;
			return true;
		}
	}
}
