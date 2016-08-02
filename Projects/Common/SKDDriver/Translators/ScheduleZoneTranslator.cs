using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OperationResult = StrazhAPI.OperationResult;

namespace StrazhDAL
{
	public class ScheduleZoneTranslator : WithFilterTranslator<DataAccess.ScheduleZone, ScheduleZone, ScheduleZoneFilter>
	{
		public ScheduleZoneTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override Expression<Func<DataAccess.ScheduleZone, bool>> IsInFilter(ScheduleZoneFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => filter.ScheduleUIDs.Contains(e.ScheduleUID));
			return result;
		}

		protected override OperationResult CanSave(ScheduleZone item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			var scheduleZones = Table.Where(x => x.ScheduleUID == item.ScheduleUID && x.UID != item.UID);
			if (scheduleZones.Any(x => x.ZoneUID == item.ZoneUID))
				return new OperationResult("Выбранная зона уже включена");
			return new OperationResult();
		}

		protected override ScheduleZone Translate(DataAccess.ScheduleZone tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.ScheduleUID = tableItem.ScheduleUID;
			apiItem.ZoneUID = tableItem.ZoneUID;
			apiItem.DoorUID = tableItem.DoorUID;
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.ScheduleZone tableItem, ScheduleZone apiItem)
		{
			var schedule = Context.Schedules.FirstOrDefault(item => item.UID == apiItem.ScheduleUID);
			if (schedule == null)
				tableItem.ScheduleUID = apiItem.ScheduleUID;
			else
				tableItem.Schedule = schedule;
			tableItem.ZoneUID = apiItem.ZoneUID;
			tableItem.DoorUID = apiItem.DoorUID;
		}

		public List<ScheduleZone> TranslateAll(IEnumerable<DataAccess.ScheduleZone> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}