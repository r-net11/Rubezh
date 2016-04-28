using System.Collections.Generic;
using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class ScheduleSchemeTranslator : OrganisationElementTranslator<DataAccess.ScheduleScheme, ScheduleScheme, ScheduleSchemeFilter>
	{
		private bool _withDays;

		public ScheduleSchemeTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
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
			if (filter.DayIntervalUIDs.Count > 0)
				result = result.And(e => e.ScheduleDays.Any(x => filter.DayIntervalUIDs.Contains(x.DayIntervalUID.Value)));
			return result;
		}

		protected override OperationResult CanSave(ScheduleScheme item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.OrganisationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name);
			if (hasSameName)
                return new OperationResult(Resources.Language.Translators.ScheduleSchemeTranslator.CanSave_Error);
			return new OperationResult();
		}

		protected override ScheduleScheme Translate(DataAccess.ScheduleScheme tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			if (_withDays)
				result.DayIntervals = DatabaseService.ScheduleDayIntervalTranslator.TranslateAll(tableItem.ScheduleDays.OrderBy(item => item.Number));
			result.Type = (ScheduleSchemeType)tableItem.Type;
			result.DaysCount = tableItem.DaysCount;
			return result;
		}

		protected override void TranslateBack(DataAccess.ScheduleScheme tableItem, ScheduleScheme apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.Type = (int)apiItem.Type;
			tableItem.DaysCount = apiItem.DaysCount;
			DatabaseService.ScheduleDayIntervalTranslator.Save(apiItem.DayIntervals);
		}

		public IEnumerable<ScheduleScheme> TranslateAll(IEnumerable<DataAccess.ScheduleScheme> scheduleSchemes)
		{
			return scheduleSchemes.Select(item => Translate(item)).ToList();
		}

		public IEnumerable<ScheduleScheme> GetScheduleSchemes(DayInterval dayInterval)
		{
			var scheduleSchemesUids = Context.ScheduleDays.Where(x => x.DayIntervalUID == dayInterval.UID)
				.Select(y => y.ScheduleSchemeUID)
				.Distinct();
			var scheduleSchemes = Context.ScheduleSchemes.Where(x => scheduleSchemesUids.Any(y => y == x.UID));
			return DatabaseService.ScheduleSchemeTranslator.TranslateAll(scheduleSchemes);
		}

		public IEnumerable<ScheduleScheme> GetScheduleSchemes(DayIntervalPart dayIntervalPart)
		{
			var scheduleSchemesUids = Context.ScheduleDays.Where(x => x.DayIntervalUID == dayIntervalPart.DayIntervalUID)
				.Select(y => y.ScheduleSchemeUID)
				.Distinct();
			var scheduleSchemes = Context.ScheduleSchemes.Where(x => scheduleSchemesUids.Any(y => y == x.UID));
			return DatabaseService.ScheduleSchemeTranslator.TranslateAll(scheduleSchemes);
		}
	}
}