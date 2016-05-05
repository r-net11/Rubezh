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
		public static List<TimeTrackZone> GetMergedZones(ShortEmployee employee)
		{
			var schedule = ScheduleHelper.GetSingle(employee.ScheduleUID);
			if (schedule == null) return SKDManager.Zones.Select(x => new TimeTrackZone(x)).ToList();

			return SKDManager.Zones.Select(zone => schedule.Zones.Any(x => x.ZoneUID == zone.UID)
				? new TimeTrackZone(zone) { IsURV = true }
				: new TimeTrackZone(zone)).ToList();
		}

		public static DayTimeTrackPart ResolveConflictWithSettingBorders(DayTimeTrackPart originalInterval, List<DayTimeTrackPart> conflictedIntervals)
		{
			var leftConflictedInterval =
				conflictedIntervals.Where(x => x.ExitDateTime > originalInterval.EnterTimeOriginal).Min(x => x.EnterDateTime);
			var rightConflictedInterval =
				conflictedIntervals.Where(x => x.EnterDateTime > originalInterval.ExitTimeOriginal).Max(x => x.ExitDateTime);

			if (leftConflictedInterval != null)
			{
				originalInterval.ExitDateTime = leftConflictedInterval.Value;
			}

			if (rightConflictedInterval != null)
			{
				originalInterval.EnterDateTime = rightConflictedInterval.Value;
			}

			originalInterval.NotTakeInCalculations = originalInterval.TimeTrackZone != null && originalInterval.TimeTrackZone.IsURV
													? originalInterval.NotTakeInCalculationsOriginal
													: originalInterval.NotTakeInCalculations;
			originalInterval.IsNeedAdjustment = originalInterval.IsNeedAdjustmentOriginal;
			originalInterval.IsDirty = true;
			return originalInterval;
		}
	}
}
