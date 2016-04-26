using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class HolidayTranslator : OrganisationElementTranslator<DataAccess.Holiday, Holiday, HolidayFilter>
	{
		public HolidayTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override IQueryable<DataAccess.Holiday> GetQuery(HolidayFilter filter)
		{
			return base.GetQuery(filter).OrderBy(item => item.Date);
		}

		protected override Expression<Func<DataAccess.Holiday, bool>> IsInFilter(HolidayFilter filter)
		{
			var result = base.IsInFilter(filter);
			if (filter.Year != 0)
				result = result.And(e => e.Date.Year == filter.Year);
			return result;
		}

		protected override OperationResult CanSave(Holiday item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;

			return Validate(item);
		}

		protected override Holiday Translate(DataAccess.Holiday tableItem)
		{
			var apiItem = base.Translate(tableItem);
			apiItem.Date = tableItem.Date;
			apiItem.Name = tableItem.Name;
			apiItem.Reduction = TimeSpan.FromSeconds(tableItem.Reduction);
			apiItem.TransferDate = tableItem.TransferDate;
			apiItem.Type = (HolidayType)tableItem.Type;
			return apiItem;
		}

		protected override void TranslateBack(DataAccess.Holiday tableItem, Holiday apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.Date = apiItem.Date;
			tableItem.Name = apiItem.Name;
			tableItem.Reduction = (int)apiItem.Reduction.TotalSeconds;
			tableItem.TransferDate = apiItem.TransferDate < TranslatiorHelper.MinYear ? TranslatiorHelper.MinYear : apiItem.TransferDate;
			tableItem.Type = (int)apiItem.Type;
		}

		protected override OperationResult CanRestore(Guid uid)
		{
			var result = base.CanRestore(uid);
			if (result.HasError)
				return result;
			
			var holidayToRestore = Table.FirstOrDefault(x => x.UID == uid);
			if (holidayToRestore == null)
				return new OperationResult(string.Format(Resources.Language.HolidayTranslator.CanRestore_Error, uid));

			return Validate(Translate(holidayToRestore));
		}

		protected override bool IsSimilarNames(DataAccess.Holiday item1, DataAccess.Holiday item2)
		{
			// Для разных годов могут быть одинаковые названия праздников
			return base.IsSimilarNames(item1, item2) && item1.Date == item2.Date;
		}

		private OperationResult Validate(Holiday holiday)
		{
			// Для одной организации нельзя ввести два праздничных дня с одинаковым названием в пределах одного года
			if (Table.Any(x =>
				x.OrganisationUID == holiday.OrganisationUID
				&& x.UID != holiday.UID
				&& x.Name == holiday.Name
				&& x.Date.Year == holiday.Date.Year
				&& !x.IsDeleted))
				return new OperationResult(Resources.Language.HolidayTranslator.Validate_HolidayDateYear_Error);

			// Для одной организации нельзя ввести два праздничных дня на одну и ту же дату
			if (Table.Any(x =>
				x.UID != holiday.UID
				&& x.OrganisationUID == holiday.OrganisationUID
				&& x.Date.Date == holiday.Date.Date
				&& !x.IsDeleted))
				return new OperationResult(string.Format(Resources.Language.HolidayTranslator.Validate_HolidayDateDay_Error, holiday.Date.ToString("d")));

			// Для одной организации нельзя ввести два праздничных дня типа "Рабочий выходной" с одной и той же датой переноса
			if (Table.Any(x =>
				x.UID != holiday.UID
				&& x.OrganisationUID == holiday.OrganisationUID
				&& (HolidayType)x.Type == HolidayType.WorkingHoliday
				&& (HolidayType)x.Type == holiday.Type
				&& x.TransferDate == holiday.TransferDate
				&& !x.IsDeleted))
				return new OperationResult(string.Format(Resources.Language.HolidayTranslator.Validate_HolidayTransferDate, holiday.Date.ToString("d")));

			return new OperationResult();
		}
	}
}