using System;

namespace StrazhAPI.Extensions
{
	public static class TimeSpanExtensions
	{
		#region Truncating
		public static TimeSpan WithoutSeconds(this TimeSpan value)
		{
			return new TimeSpan(value.Days, value.Hours, value.Minutes, 0, value.Milliseconds);
		}

		public static TimeSpan WithoutMinutes(this TimeSpan value)
		{
			return new TimeSpan(value.Days, value.Hours, 0, value.Seconds, value.Milliseconds);
		}

		public static TimeSpan WithoutMilliseconds(this TimeSpan value)
		{
			return new TimeSpan(value.Days, value.Hours, value.Minutes, value.Seconds, 0);
		}

		public static TimeSpan WithoutDays(this TimeSpan value)
		{
			return new TimeSpan(0, value.Hours, value.Minutes, value.Seconds, value.Milliseconds);
		}
		#endregion
	}
}
