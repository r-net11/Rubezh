using System;
using System.Linq;
using FiresecAPI.EmployeeTimeIntervals;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class NamedIntervalTranslator : OrganisationElementTranslator<DataAccess.NamedInterval, NamedInterval, NamedIntervalFilter>
	{
		private TimeIntervalTranslator _timeIntervalTranslator;
		public NamedIntervalTranslator(DataAccess.SKDDataContext context, TimeIntervalTranslator timeIntervalTranslator)
			: base(context)
		{
			_timeIntervalTranslator = timeIntervalTranslator;
		}

		protected override IQueryable<DataAccess.NamedInterval> GetQuery(NamedIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}

		protected override OperationResult CanSave(NamedInterval item)
		{
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				x.IsDeleted == false);
			if (hasSameName)
				return new OperationResult("Именнованный интервал с таким же названием уже содержится в базе данных");
			return base.CanSave(item);
		}
		protected override OperationResult CanDelete(Guid uid)
		{
			if (Context.Days.Any(item => !item.IsDeleted && item.NamedIntervalUID == uid))
				return new OperationResult("Именованый интервал не может быть удален, так как имеет привязку одного из графиков");
			return base.CanDelete(uid);
		}

		protected override NamedInterval Translate(DataAccess.NamedInterval tableItem)
		{
			var result = base.Translate(tableItem);
			result.Name = tableItem.Name;
			result.Description = tableItem.Description;
			result.SlideTime = TimeSpan.FromSeconds(tableItem.SlideTime);
			result.TimeIntervals = _timeIntervalTranslator.TranslateAll(tableItem.Intervals.Where(item => !item.IsDeleted).OrderBy(item => item.BeginTime));
			return result;
		}

		protected override void TranslateBack(DataAccess.NamedInterval tableItem, NamedInterval apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.SlideTime = (int)apiItem.SlideTime.TotalSeconds;
			_timeIntervalTranslator.Save(apiItem.TimeIntervals, false);
		}
	}
}