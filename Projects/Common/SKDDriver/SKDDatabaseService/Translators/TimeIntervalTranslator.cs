using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class TimeIntervalTranslator : IsDeletedTranslator<DataAccess.Interval, TimeInterval, TimeIntervalFilter>
	{
		private int DaySeconds = 86400;
		public TimeIntervalTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override IQueryable<DataAccess.Interval> GetQuery(TimeIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.BeginTime);
		}

		protected override Expression<Func<DataAccess.Interval, bool>> IsInFilter(TimeIntervalFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => filter.NamedIntervalUIDs.Contains(e.NamedIntervalUID));
			return result;
		}

		protected override OperationResult CanSave(TimeInterval item)
		{
			if (item.BeginTime == item.EndTime)
				return new OperationResult("Интервал не может иметь нулевую длительность");
			var intervals = Table.Where(x => x.NamedIntervalUID == item.NamedIntervalUID && x.UID != item.UID && !x.IsDeleted);
			if (intervals.Count() == 0 && item.BeginTime.TotalSeconds >= DaySeconds)
				return new OperationResult("Последовательность интервалов не может начинаться со следующего дня");
			var beginTime = item.BeginTime.TotalSeconds;
			if (item.IntervalTransitionType == IntervalTransitionType.NextDay)
				beginTime += DaySeconds;
			var endTime = item.EndTime.TotalSeconds;
			if (item.IntervalTransitionType != IntervalTransitionType.Day)
				endTime += DaySeconds;
			if (beginTime > endTime)
				return new OperationResult("Время окончания интервала должно быть позже времени начала");
			foreach (var interval in intervals)
				if ((interval.BeginTime >= beginTime && interval.BeginTime <= endTime) || (interval.EndTime >= beginTime && interval.EndTime <= endTime))
					return new OperationResult("Последовательность интервалов не должна быть пересекающейся");
			return base.CanSave(item);
		}

		protected override TimeInterval Translate(DataAccess.Interval tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.NamedIntervalUID = tableItem.NamedIntervalUID;
			if (tableItem.BeginTime > DaySeconds)
			{
				apiItem.IntervalTransitionType = IntervalTransitionType.NextDay;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime - DaySeconds);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime - DaySeconds);
			}
			else if (tableItem.EndTime > DaySeconds)
			{
				apiItem.IntervalTransitionType = IntervalTransitionType.Night;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime - DaySeconds);
			}
			else
			{
				apiItem.IntervalTransitionType = IntervalTransitionType.Day;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime);
			}
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.Interval tableItem, TimeInterval apiItem)
		{
			var namedInterval = Context.NamedIntervals.FirstOrDefault(item => item.UID == apiItem.NamedIntervalUID);
			base.TranslateBack(tableItem, apiItem);
			tableItem.NamedIntervalUID = apiItem.NamedIntervalUID;
			if (namedInterval != null)
				tableItem.NamedInterval = namedInterval;
			tableItem.BeginTime = (int)apiItem.BeginTime.TotalSeconds;
			tableItem.EndTime = (int)apiItem.EndTime.TotalSeconds;
			switch (apiItem.IntervalTransitionType)
			{
				case IntervalTransitionType.NextDay:
					tableItem.BeginTime += DaySeconds;
					tableItem.EndTime += DaySeconds;
					break;
				case IntervalTransitionType.Night:
					tableItem.EndTime += DaySeconds;
					break;
			}
		}

		public List<TimeInterval> TranslateAll(IEnumerable<DataAccess.Interval> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}
