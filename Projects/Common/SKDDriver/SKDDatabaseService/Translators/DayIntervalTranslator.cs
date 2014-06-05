using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class DayIntervalTranslator : IsDeletedTranslator<DataAccess.Day, DayInterval, DayIntervalFilter>
	{
		public DayIntervalTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override IQueryable<DataAccess.Day> GetQuery(DayIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Number);
		}
		protected override Expression<Func<DataAccess.Day, bool>> IsInFilter(DayIntervalFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => filter.ScheduleSchemeUIDs.Contains(e.ScheduleSchemeUID));
			return result;
		}
		public override OperationResult MarkDeleted(IEnumerable<DayInterval> items)
		{
			foreach (var dayInterval in items)
			{
				var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(item => item.UID == dayInterval.ScheduleSchemeUID);
				if (scheduleScheme != null)
					foreach (var day in scheduleScheme.Days)
						if (day.Number > dayInterval.Number)
							day.Number--;
			}
			return base.MarkDeleted(items);
		}

		protected override OperationResult CanSave(DayInterval item)
		{
			// проверить что в результате нет пересечений рабочего времени ИменованногоИнтервала с предыдущим и следующим днем
			return base.CanSave(item);
		}

		protected override DayInterval Translate(DataAccess.Day tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.NamedIntervalUID = tableItem.NamedIntervalUID.HasValue ? tableItem.NamedIntervalUID.Value : Guid.Empty;
			apiItem.Number = tableItem.Number;
			apiItem.ScheduleSchemeUID = tableItem.ScheduleSchemeUID;
			return apiItem;
		}
		protected override void TranslateBack(DataAccess.Day tableItem, DayInterval apiItem)
		{
			var namedInterval = apiItem.NamedIntervalUID == Guid.Empty ? null : Context.NamedIntervals.FirstOrDefault(item => item.UID == apiItem.NamedIntervalUID);
			var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(item => item.UID == apiItem.ScheduleSchemeUID);
			base.TranslateBack(tableItem, apiItem);
			if (namedInterval == null && apiItem.NamedIntervalUID != Guid.Empty)
				tableItem.NamedIntervalUID = apiItem.NamedIntervalUID;
			else
				tableItem.NamedInterval = namedInterval;
			if (scheduleScheme == null)
				tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID;
			else
				tableItem.ScheduleScheme = scheduleScheme;
			tableItem.Number = apiItem.Number;
		}

		public List<DayInterval> TranslateAll(IEnumerable<DataAccess.Day> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}
