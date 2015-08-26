using System;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;
using System.Collections.Generic;

namespace SKDDriver.DataClasses
{
	public class DayIntervalTranslator : OrganisationItemTranslatorBase<DayInterval, API.DayInterval, API.DayIntervalFilter>
	{
		TimeSpan _daySeconds = new TimeSpan(0, 23, 59, 59, 59);// 86400;

		public DayIntervalTranslator(DbService context)
			: base(context)
		{
			AsyncTranslator = new DayIntervalAsyncTranslator(this);
		}

		public DayIntervalAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<DayInterval> Table
		{
			get { return Context.DayIntervals; }
		}

		public override IQueryable<DayInterval> GetTableItems()
		{
			return base.GetTableItems().Include(x => x.DayIntervalParts);
		}

		public override void TranslateBack(API.DayInterval apiItem, DayInterval tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.SlideTimeSpan = apiItem.SlideTime;
			tableItem.DayIntervalParts = apiItem.DayIntervalParts.Select(x => TranslatePartBack(x)).ToList();
		}

		DayIntervalPart TranslatePartBack(API.DayIntervalPart apiItem)
		{
			var tableItem = new DayIntervalPart();
			tableItem.UID = apiItem.UID;
			tableItem.BeginTimeSpan = apiItem.BeginTime;
			tableItem.EndTimeSpan = apiItem.EndTime;
			tableItem.Number = apiItem.Number;
			return tableItem;
		}

		protected override OperationResult<bool> CanSave(API.DayInterval dayInterval)
		{
			if (dayInterval == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (dayInterval.UID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			bool hasSameName = Table.Any(x => x.OrganisationUID==dayInterval.OrganisationUID &&
				!x.IsDeleted &&
				x.Name == dayInterval.Name &&
				x.OrganisationUID == dayInterval.OrganisationUID &&
				x.UID != dayInterval.UID);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			var intervals = dayInterval.DayIntervalParts;
			foreach (var item in intervals)
			{
				var beginTime = item.BeginTime;
				var endTime = item.EndTime;
				if (item.TransitionType != API.DayIntervalPartTransitionType.Day)
					endTime = endTime.Add(_daySeconds);
				if (beginTime == endTime)
					return OperationResult<bool>.FromError("Интервал не может иметь нулевую длительность");
				var otherIntervals = intervals.Where(x => x.UID != item.UID);
				if (otherIntervals.Count() == 0 && item.BeginTime >= _daySeconds)
					return OperationResult<bool>.FromError("Последовательность интервалов не может начинаться со следующего дня");
				if (beginTime > endTime)
					return OperationResult<bool>.FromError("Время окончания интервала должно быть позже времени начала");
				foreach (var interval in otherIntervals)
				{
					var otherBeginTime = interval.BeginTime;
					var otherEndTime = interval.EndTime;
					if (interval.TransitionType!=API.DayIntervalPartTransitionType.Day)
						otherEndTime = otherEndTime.Add(_daySeconds);
					if ((otherBeginTime >= beginTime && otherBeginTime <= endTime) || (otherEndTime >= beginTime && otherEndTime <= endTime))
						return OperationResult<bool>.FromError("Последовательность интервалов не должна быть пересекающейся");
				}
			}
			return new OperationResult<bool>(true);
		}

		protected override void ClearDependentData(DayInterval tableItem)
		{
			Context.DayIntervalParts.RemoveRange(tableItem.DayIntervalParts);
		}

		protected override IEnumerable<API.DayInterval> GetAPIItems(IQueryable<DayInterval> tableItems)
		{
			return tableItems.Select(tableItem => new API.DayInterval
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				SlideTime = tableItem.SlideTimeSpan,
				DayIntervalParts = tableItem.DayIntervalParts.Select(x => new API.DayIntervalPart
				{
					UID = x.UID,
					DayIntervalUID = x.DayIntervalUID.Value,
					BeginTime = x.BeginTimeSpan,
					Number = x.Number,
					TransitionType = x.EndTimeSpan < x.BeginTimeSpan  ? API.DayIntervalPartTransitionType.Night : API.DayIntervalPartTransitionType.Day,
					EndTime = x.EndTimeSpan
				}).OrderBy(x => x.Number).ToList()
			});
		}
	}

	public class DayIntervalAsyncTranslator : AsyncTranslator<DayInterval, API.DayInterval, API.DayIntervalFilter>
	{
		public DayIntervalAsyncTranslator(DayIntervalTranslator translator) : base(translator as ITranslatorGet<DayInterval, API.DayInterval, API.DayIntervalFilter>) { }
		public override List<API.DayInterval> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.DayIntervals;
		}
	}
}