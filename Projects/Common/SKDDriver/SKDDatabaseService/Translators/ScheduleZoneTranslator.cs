using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleZoneTranslator : IsDeletedTranslator<DataAccess.ScheduleZoneLink, ScheduleZone, ScheduleZoneFilter>
	{
		public ScheduleZoneTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}
		protected override Expression<Func<DataAccess.ScheduleZoneLink, bool>> IsInFilter(ScheduleZoneFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.ScheduleZoneLink>();
			result = result.And(base.IsInFilter(filter));
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

		protected override ScheduleZone Translate(DataAccess.ScheduleZoneLink tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.ScheduleUID = tableItem.ScheduleUID;
			apiItem.ZoneUID = tableItem.ZoneUID;
			apiItem.IsControl = tableItem.IsControl;
			return apiItem;
		}
		protected override void TranslateBack(DataAccess.ScheduleZoneLink tableItem, ScheduleZone apiItem)
		{
			var schedule = Context.Schedules.FirstOrDefault(item => item.UID == apiItem.ScheduleUID);
			base.TranslateBack(tableItem, apiItem);
			if (schedule == null)
				tableItem.ScheduleUID = apiItem.ScheduleUID;
			else
				tableItem.Schedule = schedule;
			tableItem.ZoneUID = apiItem.ZoneUID;
			tableItem.IsControl = apiItem.IsControl;
		}

		public List<ScheduleZone> TranslateAll(IEnumerable<DataAccess.ScheduleZoneLink> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}
