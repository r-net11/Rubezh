using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OperationResult = StrazhAPI.OperationResult;

namespace StrazhDAL
{
	public class ScheduleDayIntervalTranslator : WithFilterTranslator<DataAccess.ScheduleDay, ScheduleDayInterval, ScheduleDayIntervalFilter>
	{
		public ScheduleDayIntervalTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override IQueryable<DataAccess.ScheduleDay> GetQuery(ScheduleDayIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Number);
		}

		protected override Expression<Func<DataAccess.ScheduleDay, bool>> IsInFilter(ScheduleDayIntervalFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => e.ScheduleSchemeUID.Equals(filter.ScheduleSchemeUID));
			return result;
		}

		public override OperationResult Delete(Guid uid)
		{
			var dayInterval = Table.FirstOrDefault(x => x.UID == uid);
			if (dayInterval != null)
			{
				var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(item => item.UID == dayInterval.ScheduleSchemeUID);
				if (scheduleScheme != null)
					foreach (var day in scheduleScheme.ScheduleDays)
						if (day.Number > dayInterval.Number)
							day.Number--;
			}
			return base.Delete(uid);
		}

		protected override OperationResult CanSave(ScheduleDayInterval item)
		{
			// проверить что в результате нет пересечений рабочего времени ИменованногоИнтервала с предыдущим и следующим днем
			return base.CanSave(item);
		}

		protected override ScheduleDayInterval Translate(DataAccess.ScheduleDay tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.DayIntervalUID = tableItem.DayIntervalUID.HasValue ? tableItem.DayIntervalUID.Value : Guid.Empty;
			apiItem.Number = tableItem.Number;
			apiItem.ScheduleSchemeUID = tableItem.ScheduleSchemeUID;
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.ScheduleDay tableItem, ScheduleDayInterval apiItem)
		{
			var dayInterval = apiItem.DayIntervalUID == Guid.Empty ? null : Context.DayIntervals.FirstOrDefault(item => item.UID == apiItem.DayIntervalUID);
			var scheduleScheme = Context.ScheduleSchemes.FirstOrDefault(item => item.UID == apiItem.ScheduleSchemeUID);
			if (dayInterval == null && apiItem.DayIntervalUID != Guid.Empty)
				tableItem.DayIntervalUID = apiItem.DayIntervalUID;
			else
				tableItem.DayInterval = dayInterval;
			if (scheduleScheme == null)
				tableItem.ScheduleSchemeUID = apiItem.ScheduleSchemeUID;
			else
				tableItem.ScheduleScheme = scheduleScheme;
			tableItem.Number = apiItem.Number;
		}

		public List<ScheduleDayInterval> TranslateAll(IEnumerable<DataAccess.ScheduleDay> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}