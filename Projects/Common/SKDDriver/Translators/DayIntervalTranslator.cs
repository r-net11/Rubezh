using System;
using System.Linq;
using FiresecAPI.SKD;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class DayIntervalTranslator : OrganisationElementTranslator<DataAccess.DayInterval, DayInterval, DayIntervalFilter>
	{
		public DayIntervalTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override IQueryable<DataAccess.DayInterval> GetQuery(DayIntervalFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Name);
		}

		protected override OperationResult CanSave(DayInterval item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return new OperationResult("Дневной график с таким же названием уже существует");
			else
				return new OperationResult();
		
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
			result.DayIntervalParts = DatabaseService.DayIntervalPartTranslator.TranslateAll(tableItem.DayIntervalParts.Where(item => !item.IsDeleted).OrderBy(item => item.BeginTime));
			return result;
		}

		protected override void TranslateBack(DataAccess.DayInterval tableItem, DayInterval apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.SlideTime = (int)apiItem.SlideTime.TotalSeconds;
			DatabaseService.DayIntervalPartTranslator.Save(apiItem.DayIntervalParts);
		}
	}
}