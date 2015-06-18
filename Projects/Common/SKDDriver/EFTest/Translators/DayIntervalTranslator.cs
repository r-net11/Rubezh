using System;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class DayIntervalTranslator : OrganisationItemTranslatorBase<DayInterval, API.DayInterval, API.DayIntervalFilter>
	{
		int _daySeconds = 86400;

		public DayIntervalTranslator(DbService context) : base(context) { }
		
		protected override DbSet<DayInterval> Table
		{
			get { return Context.DayIntervals; }
		}

		protected override IQueryable<DayInterval> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.DayIntervalParts);
		}

		public override API.DayInterval Translate(DayInterval tableItem)
		{
			var result = base.Translate(tableItem);
			result.SlideTime = TimeSpan.FromTicks(tableItem.SlideTime);
			result.DayIntervalParts = tableItem.DayIntervalParts.Select(x => TranslatePart(x)).ToList();
			return result;
		}

		public override void TranslateBack(API.DayInterval apiItem, DayInterval tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.SlideTime = (int)apiItem.SlideTime.TotalSeconds;
			tableItem.DayIntervalParts = apiItem.DayIntervalParts.Select(x => TranslatePartBack(x)).ToList();
		}

		API.DayIntervalPart TranslatePart(DayIntervalPart tableItem)
		{
			var apiItem = new API.DayIntervalPart { UID = tableItem.UID, DayIntervalUID = tableItem.DayIntervalUID.Value };
			if (tableItem.EndTime > _daySeconds)
			{
				apiItem.TransitionType = API.DayIntervalPartTransitionType.Night;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime - _daySeconds);
			}
			else
			{
				apiItem.TransitionType = API.DayIntervalPartTransitionType.Day;
				apiItem.BeginTime = TimeSpan.FromSeconds(tableItem.BeginTime);
				apiItem.EndTime = TimeSpan.FromSeconds(tableItem.EndTime);
			}
			return apiItem;
		}

		DayIntervalPart TranslatePartBack(API.DayIntervalPart apiItem)
		{
			var tableItem = new DayIntervalPart();
			tableItem.UID = apiItem.UID;
			tableItem.BeginTime = (int)apiItem.BeginTime.TotalSeconds;
			tableItem.EndTime = (int)apiItem.EndTime.TotalSeconds;
			if (apiItem.TransitionType == API.DayIntervalPartTransitionType.Night)
			{
				tableItem.EndTime += _daySeconds;
			}
			return tableItem;
		}

		protected override OperationResult<bool> CanSave(API.DayInterval dayInterval)
		{
			var baseResult = base.CanSave(dayInterval);
			if (baseResult.HasError)
				return baseResult;
			var intervals = dayInterval.DayIntervalParts;
			foreach (var item in intervals)
			{
				var beginTime = item.BeginTime;
				var endTime = item.EndTime;
				if (item.TransitionType != API.DayIntervalPartTransitionType.Day)
					endTime.Add(TimeSpan.FromSeconds(_daySeconds));
				if (beginTime == endTime)
					return OperationResult<bool>.FromError("Интервал не может иметь нулевую длительность");
				var otherIntervals = intervals.Where(x => x.UID != item.UID);
				if (otherIntervals.Count() == 0 && item.BeginTime.TotalSeconds >= _daySeconds)
					return OperationResult<bool>.FromError("Последовательность интервалов не может начинаться со следующего дня");
				if (beginTime > endTime)
					return OperationResult<bool>.FromError("Время окончания интервала должно быть позже времени начала");
				foreach (var interval in otherIntervals)
					if ((interval.BeginTime >= beginTime && interval.BeginTime <= endTime) || (interval.EndTime >= beginTime && interval.EndTime <= endTime))
						return OperationResult<bool>.FromError("Последовательность интервалов не должна быть пересекающейся");
			}
			return new OperationResult<bool>(true);
		}

		protected override void ClearDependentData(DayInterval tableItem)
		{
			Context.DayIntervalParts.RemoveRange(tableItem.DayIntervalParts);
		}
	}
}
