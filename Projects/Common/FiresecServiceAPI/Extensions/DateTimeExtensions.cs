using System;
using System.Globalization;

namespace StrazhAPI.Extensions
{
	public static class DateTimeExtensions
	{
		public static DateTime PreviosStartMonth(this DateTime date)
		{
			var startOfTheMonth = new DateTime(date.Year, date.Month, 1)
			.AddMonths(-1);

			return startOfTheMonth;
		}

		public static DateTime PreviosEndMonth(this DateTime date)
		{
			var endOfTheMonth = new DateTime(date.Year, date.Month, 1)
				.AddDays(-1);

			return endOfTheMonth;
		}

		public static DateTime PreviosWeekMonday(this DateTime date)
		{
			const int daysInWeek = 7;
			const int countOfMonday = 1;
			var thisWeekMonday = -((int)date.DayOfWeek) + countOfMonday;

			return new GregorianCalendar().AddDays(date, thisWeekMonday - daysInWeek);
		}

		public static DateTime PreviosWeekSunday(this DateTime date)
		{
			const int daysInWeek = 7;
			const int countOfSunday = 7;
			var thisWeekSunday = -((int)date.DayOfWeek) + countOfSunday;

			return new GregorianCalendar().AddDays(date, thisWeekSunday - daysInWeek);
		}

		public static DateTime PreviosWeekFriday(this DateTime date)
		{
			const int daysInWeek = 7;
			const int countOfFriday = 5;
			var thisWeekFriday = -((int)date.DayOfWeek) + countOfFriday;

			return new GregorianCalendar().AddDays(date, thisWeekFriday - daysInWeek);
		}

		public static DateTime PreviosStartDay(this DateTime date)
		{
			return date.Date.AddDays(-1);
		}

		public static DateTime PreviosEndDay(this DateTime date)
		{
			return date.Date.AddSeconds(-1);
		}

		public static DateTime ResetTime(this DateTime date)
		{
			return new DateTime(date.Year, date.Month, date.Day);
		}

		public static DateTime NextDay(this DateTime date)
		{
			return date.AddDays(1);
		}

		public static DateTime NextWeek(this DateTime date)
		{
			const int daysInWeek = 7;
			return date.AddDays(daysInWeek);
		}

		public static DateTime NextMonth(this DateTime date)
		{
			return date.AddMonths(1);
		}
	}
}
