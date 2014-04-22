using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using OperationResult = FiresecAPI.OperationResult;
using System;
using LinqKit;

namespace SKDDriver.Translators
{
	public class ScheduleSchemeTranslator : OrganizationElementTranslator<DataAccess.ScheduleScheme, ScheduleScheme, ScheduleSchemeFilter>
	{
		private DayIntervalTranslator _dayIntervalTranslator;
		public ScheduleSchemeTranslator(DataAccess.SKDDataContext context, DayIntervalTranslator dayIntervalTranslator)
			: base(context)
		{
			_dayIntervalTranslator = dayIntervalTranslator;
		}

		protected override IQueryable<DataAccess.ScheduleScheme> GetQuery(ScheduleSchemeFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}
		protected override System.Linq.Expressions.Expression<Func<DataAccess.ScheduleScheme, bool>> IsInFilter(ScheduleSchemeFilter filter)
		{
			var result = PredicateBuilder.True<DataAccess.ScheduleScheme>();
			result = result.And(base.IsInFilter(filter));
			var type = (int)filter.Type;
			if (type != 0)
				result = result.And(e => (type & e.Type) == e.Type);
			return result;
		}

		protected override OperationResult CanSave(ScheduleScheme item)
		{
			bool sameName = Table.Any(x => x.OrganizationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name);
			if (sameName)
				return new OperationResult("График с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}

		protected override ScheduleScheme Translate(DataAccess.ScheduleScheme tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
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
