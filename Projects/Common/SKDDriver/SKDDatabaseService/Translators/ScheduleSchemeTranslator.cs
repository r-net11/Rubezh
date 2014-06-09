using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleSchemeTranslator : OrganisationElementTranslator<DataAccess.ScheduleScheme, ScheduleScheme, ScheduleSchemeFilter>
	{
		private DayIntervalTranslator _dayIntervalTranslator;
		private bool _withDays;
		public ScheduleSchemeTranslator(DataAccess.SKDDataContext context, DayIntervalTranslator dayIntervalTranslator)
			: base(context)
		{
			_dayIntervalTranslator = dayIntervalTranslator;
		}

		protected override IQueryable<DataAccess.ScheduleScheme> GetQuery(ScheduleSchemeFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}
		protected override Expression<Func<DataAccess.ScheduleScheme, bool>> IsInFilter(ScheduleSchemeFilter filter)
		{
			_withDays = filter.WithDays;
			var result = base.IsInFilter(filter);
			var type = (int)filter.Type;
			if (type != 0)
				result = result.And(e => (type & e.Type) == e.Type);
			return result;
		}

		protected override OperationResult CanSave(ScheduleScheme item)
		{
			bool sameName = Table.Any(x => x.OrganisationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name);
			if (sameName)
				return new OperationResult("График с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}
		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Schedules.Any(item => !item.IsDeleted && item.ScheduleSchemeUID == uid))
				return new OperationResult("Нельзя удалить режим работы, так как он привязан к одному из графиков");
			return base.CanDelete(uid);
		}

		protected override ScheduleScheme Translate(DataAccess.ScheduleScheme tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			if (_withDays)
				result.DayIntervals = _dayIntervalTranslator.TranslateAll(tableItem.Days.Where(item => !item.IsDeleted).OrderBy(item => item.Number));
			result.Type = (ScheduleSchemeType)tableItem.Type;
			return result;
		}

		protected override void TranslateBack(DataAccess.ScheduleScheme tableItem, ScheduleScheme apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.Type = (int)apiItem.Type;
			_dayIntervalTranslator.Save(apiItem.DayIntervals, false);
		}
	}
}
