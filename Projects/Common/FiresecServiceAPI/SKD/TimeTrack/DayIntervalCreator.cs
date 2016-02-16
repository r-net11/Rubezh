using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FiresecAPI.SKD
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
				Name = "Никогда",
			};
		}
	}
}
