using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using SKDModule.Model;
using DayTimeTrackPart = SKDModule.Model.DayTimeTrackPart;
using TimeTrackZone = SKDModule.Model.TimeTrackZone;

namespace SKDModule.Helpers
{
	public static class TimeTrackingHelper
	{
		/// <summary>
		/// Получает список всех зон (урв и неурв)
		/// </summary>
		/// <param name="employee">Сотрудник, для которого производится получение всех доступных для него зон</param>
		/// <returns>Доступные УРВ и неУРВ зоны</returns>
		public static List<TimeTrackZone> GetAllZones(ShortEmployee employee)
		{
			var schedule = ScheduleHelper.GetSingle(employee.ScheduleUID);
			if (schedule == null) return SKDManager.Zones.Select(x => new TimeTrackZone(x)).ToList();

			return SKDManager.Zones.Select(zone => schedule.Zones.Any(x => x.ZoneUID == zone.UID)
				? new TimeTrackZone(zone) { IsURV = true }
				: new TimeTrackZone(zone)).ToList();
		}
	}
}
