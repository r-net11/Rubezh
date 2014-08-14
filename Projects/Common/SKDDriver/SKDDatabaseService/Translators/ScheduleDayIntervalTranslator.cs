using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleDayIntervalTranslator : IsDeletedTranslator<DataAccess.Day, ScheduleDayInterval, ScheduleDayIntervalFilter>
	{
		public ScheduleDayIntervalTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override IQueryable<DataAccess.Day> GetQuery(ScheduleDayIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Number);
		}
		protected override Expression<Func<DataAccess.Day, bool>> IsInFilter(ScheduleDayIntervalFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => e.ScheduleSchemeUID.Equals(filter.ScheduleSchemeUID));
			return result;
		}
		public override OperationResult MarkDeleted(Guid uid)
		{
			var dayInterval = Table.FirstOrDefault(x => x.UID == uid);
			if (dayInterval != null)
			{
				var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(item => item.UID == dayInterval.ScheduleSchemeUID);
				if (scheduleScheme != null)
					foreach (var day in scheduleScheme.Days)
						if (day.Number > dayInterval.Number)
							day.Number--;
			}
			return base.MarkDeleted(uid);
		}

		protected override OperationResult CanSave(ScheduleDayInterval item)
		{
			// проверить что в результате нет пересечений рабочего времени ИменованногоИнтервала с предыдущим и следующим днем
			return base.CanSave(item);
		}

		protected override ScheduleDayInterval Translate(DataAccess.Day tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.DayIntervalUID = tableItem.NamedIntervalUID.HasValue ? tableItem.NamedIntervalUID.Value : Guid.Empty;
			apiItem.Number = tableItem.Number;
			apiItem.ScheduleSchemeUID = tableItem.ScheduleSchemeUID;
			return apiItem;
		}
		protected override void TranslateBack(DataAccess.Day tableItem, ScheduleDayInterval apiItem)
		{
			var namedInterval = apiItem.DayIntervalUID == Guid.Empty ? null : Context.NamedIntervals.FirstOrDefault(item => item.UID == apiItem.DayIntervalUID);
			var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(item => item.UID == apiItem.ScheduleSchemeUID);
			base.TranslateBack(tableItem, apiItem);
			if (namedInterval == null && apiItem.DayIntervalUID != Guid.Empty)
				tableItem.NamedIntervalUID = apiItem.DayIntervalUID;
			else
				tableItem.NamedInterval = namedInterval;
			if (scheduleScheme == null)
				tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID;
			else
				tableItem.ScheduleScheme = scheduleScheme;
			tableItem.Number = apiItem.Number;
		}

		public List<ScheduleDayInterval> TranslateAll(IEnumerable<DataAccess.Day> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}