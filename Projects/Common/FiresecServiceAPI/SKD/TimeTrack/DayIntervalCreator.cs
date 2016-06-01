using System;

namespace StrazhAPI.SKD
{
	public static class DayIntervalCreator
	{
		/// <summary>
		/// Создает дневной график "Никогда"
		/// </summary>
		/// <returns>Дневной график "Никогда"</returns>
		public static DayInterval CreateDayIntervalNever()
		{
			return new DayInterval
			{
				UID = Guid.Empty,
				Name = Resources.Language.SKD.TimeTrack.DayIntervalCreator.Name,
			};
		}
	}
}
