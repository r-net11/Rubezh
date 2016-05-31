using StrazhAPI.SKD;

namespace StrazhAPI.Extensions
{
	public static class TimeTrackPartExtensions
	{
		/// <summary>
		/// Проверяет, пересекаются ли временные интервалы
		/// </summary>
		/// <param name="first">Первый интервал</param>
		/// <param name="second">Второй интервал</param>
		/// <returns>true - интервалы пересекаются. false - интервалы не пересекаются</returns>
		public static bool IsIntersect(this TimeTrackPart first, TimeTrackPart second)
		{
			if (first == null || second == null) return false;

			return first.EnterDateTime <= second.ExitDateTime
				&& first.ExitDateTime >= second.EnterDateTime;
		}

		/// <summary>
		/// Проверяет, пересекаются ли время временных интервалов
		/// </summary>
		/// <param name="first">Первый интервал</param>
		/// <param name="second">Второй интервал</param>
		/// <returns>true - интервалы пересекаются. false - интервалы не пересекаются</returns>
		public static bool IsIntersectTimeOfDay(this TimeTrackPart first, TimeTrackPart second)
		{
			if (first == null || !first.ExitDateTime.HasValue || second == null || !second.ExitDateTime.HasValue) return false;

			return first.EnterDateTime.TimeOfDay <= second.ExitDateTime.Value.TimeOfDay
				&& first.ExitDateTime.Value.TimeOfDay >= second.EnterDateTime.TimeOfDay;
		}
	}
}
