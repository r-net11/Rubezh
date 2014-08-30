using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.SKD;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleZoneTranslator : IsDeletedTranslator<DataAccess.ScheduleZone, ScheduleZone, ScheduleZoneFilter>
	{
		public ScheduleZoneTranslator(DataAccess.SKDDataContext context)
			: base(context)
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
			var scheduleZones = Table.Where(x => x.ScheduleUID == item.ScheduleUID && x.UID != item.UID && !x.IsDeleted);
			if (scheduleZones.Any(x => x.ZoneUID == item.ZoneUID))
				return new OperationResult("Выбранная зона уже включена");
			return base.CanSave(item);
		}

		protected override ScheduleZone Translate(DataAccess.ScheduleZone tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.ScheduleUID = tableItem.ScheduleUID;
			apiItem.ZoneUID = tableItem.ZoneUID;
			return apiItem;
		}
		protected override void TranslateBack(DataAccess.ScheduleZone tableItem, ScheduleZone apiItem)
		{
			var schedule = Context.Schedules.FirstOrDefault(item => item.UID == apiItem.ScheduleUID);
			base.TranslateBack(tableItem, apiItem);
			if (schedule == null)
				tableItem.ScheduleUID = apiItem.ScheduleUID;
			else
				tableItem.Schedule = schedule;
			tableItem.ZoneUID = apiItem.ZoneUID;
		}

		public List<ScheduleZone> TranslateAll(IEnumerable<DataAccess.ScheduleZone> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}