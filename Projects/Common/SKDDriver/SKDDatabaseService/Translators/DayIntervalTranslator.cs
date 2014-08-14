using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class DayIntervalTranslator : OrganisationElementTranslator<DataAccess.DayInterval, DayInterval, DayIntervalFilter>
	{
		private DayIntervalPartTranslator _timeIntervalTranslator;
		public DayIntervalTranslator(DataAccess.SKDDataContext context, DayIntervalPartTranslator timeIntervalTranslator)
			: base(context)
		{
			_timeIntervalTranslator = timeIntervalTranslator;
		}

		protected override IQueryable<DataAccess.DayInterval> GetQuery(DayIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}

		protected override OperationResult CanSave(DayInterval item)
		{
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (hasSameName)
				return new OperationResult("Дневной график с таким же названием уже существует");
			return base.CanSave(item);
		}
		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.ScheduleDays.Any(item => !item.IsDeleted && item.DayIntervalUID == uid))
				return new OperationResult("Дневной график не может быть удален, так как он содержится в одном из графиков");
			return base.CanDelete(uid);
		}

		protected override DayInterval Translate(DataAccess.DayInterval tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.SlideTime = TimeSpan.FromSeconds(tableItem.SlideTime);
			result.DayIntervalParts = _timeIntervalTranslator.TranslateAll(tableItem.DayIntervalParts.Where(item => !item.IsDeleted).OrderBy(item => item.BeginTime));
			return result;
		}

		protected override void TranslateBack(DataAccess.DayInterval tableItem, DayInterval apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.SlideTime = (int)apiItem.SlideTime.TotalSeconds;
			_timeIntervalTranslator.Save(apiItem.DayIntervalParts);
		}
	}
}