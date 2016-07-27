using System;

namespace StrazhAPI.SKD
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
			var beginTime = dayIntervalPart.BeginTime;
			var endTime = dayIntervalPart.TransitionType == DayIntervalPartTransitionType.Night
				? dayIntervalPart.EndTime.Add(TimeSpan.FromDays(1))
				: dayIntervalPart.EndTime;

			return endTime == beginTime;
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

			if (endTime <= otherBeginTime || beginTime >= otherEndTime)
				return false;
			return true;
		}

		public static DayIntervalPart GetDayIntervalPartInNextDayInterval(this DayIntervalPart dayIntervalPart)
		{
			if (dayIntervalPart.TransitionType != DayIntervalPartTransitionType.Night)
				// Временной интервал без перехода. На следующий день ничего не переходит.
				return null;

			return new DayIntervalPart
			{
				BeginTime = new TimeSpan(0, 0, 0),
				EndTime = new TimeSpan(dayIntervalPart.EndTime.Hours, dayIntervalPart.EndTime.Minutes, dayIntervalPart.EndTime.Seconds),
				TransitionType = DayIntervalPartTransitionType.Day
			};
		}
	}
}
