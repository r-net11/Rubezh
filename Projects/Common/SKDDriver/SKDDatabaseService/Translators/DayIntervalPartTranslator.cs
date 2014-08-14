using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class DayIntervalPartTranslator : IsDeletedTranslator<DataAccess.DayIntervalPart, DayIntervalPart, DayIntervalPartFilter>
	{
		int DaySeconds = 86400;
		public DayIntervalPartTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override IQueryable<DataAccess.DayIntervalPart> GetQuery(DayIntervalPartFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.BeginTime);
		}

		protected override Expression<Func<DataAccess.DayIntervalPart, bool>> IsInFilter(DayIntervalPartFilter filter)
		{
			var result = base.IsInFilter(filter);
			result = result.And(e => filter.DayIntervalUIDs.Contains(e.DayIntervalUID));
			return result;
		}

		protected override OperationResult CanSave(DayIntervalPart item)
		{
			if (item.BeginTime == item.EndTime)
				return new OperationResult("Интервал не может иметь нулевую длительность");
			var intervals = Table.Where(x => x.DayIntervalUID == item.DayIntervalUID && x.UID != item.UID && !x.IsDeleted);
			if (intervals.Count() == 0 && item.BeginTime.TotalSeconds >= DaySeconds)
				return new OperationResult("Последовательность интервалов не может начинаться со следующего дня");
			var beginTime = item.BeginTime.TotalSeconds;
			var endTime = item.EndTime.TotalSeconds;
			if (item.TransitionType != DayIntervalPartTransitionType.Day)
				endTime += DaySeconds;
			if (beginTime > endTime)
				return new OperationResult("Время окончания интервала должно быть позже времени начала");
			foreach (var interval in intervals)
				if ((interval.BeginTime >= beginTime && interval.BeginTime <= endTime) || (interval.EndTime >= beginTime && interval.EndTime <= endTime))
					return new OperationResult("Последовательность интервалов не должна быть пересекающейся");
			return base.CanSave(item);
		}

		protected override DayIntervalPart Translate(DataAccess.DayIntervalPart tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.DayIntervalUID = tableItem.DayIntervalUID;
			if (tableItem.EndTime > DaySeconds)
			{
				apiItem.TransitionType = DayIntervalPartTransitionType.Night;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime - DaySeconds);
			}
			else
			{
				apiItem.TransitionType = DayIntervalPartTransitionType.Day;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime);
			}
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.DayIntervalPart tableItem, DayIntervalPart apiItem)
		{
			var dayInterval = Context.DayIntervals.FirstOrDefault(item => item.UID == apiItem.DayIntervalUID);
			base.TranslateBack(tableItem, apiItem);
			tableItem.DayIntervalUID = apiItem.DayIntervalUID;
			if (dayInterval != null)
				tableItem.DayInterval = dayInterval;
			tableItem.BeginTime = (int)apiItem.BeginTime.TotalSeconds;
			tableItem.EndTime = (int)apiItem.EndTime.TotalSeconds;
			if (apiItem.TransitionType == DayIntervalPartTransitionType.Night)
			{
				tableItem.EndTime += DaySeconds;
			}
		}

		public List<DayIntervalPart> TranslateAll(IEnumerable<DataAccess.DayIntervalPart> list)
		{
			return list.Select(item => Translate(item)).ToList();
		}
	}
}