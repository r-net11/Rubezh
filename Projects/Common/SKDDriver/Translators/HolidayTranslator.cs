using System.Globalization;
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

			// Для одной организации нельзя ввести два праздничных дня с одинаковым названием в пределах одного года
			if (Table.Any(x => 
				x.OrganisationUID == item.OrganisationUID
				&& x.UID != item.UID
				&& x.Name == item.Name
				&& x.Date.Year == item.Date.Year
				&& !x.IsDeleted))
				return new OperationResult("Праздничный день с таким названием уже существует");

			// Для одной организации нельзя ввести два праздничных дня на одну и ту же дату
			if (Table.Any(x =>
				x.UID != item.UID
				&& x.OrganisationUID == item.OrganisationUID
				&& x.Date.Date == item.Date.Date
				&& !x.IsDeleted))
				return new OperationResult(String.Format("Праздничный день введенный на дату {0} уже существует", item.Date.ToString("d")));

			// Для одной организации нельзя ввести два праздничных дня типа "Рабочий выходной" с одной и той же датой переноса
			if (Table.Any(x =>
				x.UID != item.UID
				&& x.OrganisationUID == item.OrganisationUID
				&& (HolidayType)x.Type == HolidayType.WorkingHoliday
				&& (HolidayType)x.Type == item.Type
				&& x.TransferDate == item.TransferDate
				&& !x.IsDeleted))
				return new OperationResult(String.Format("Рабочий выходной, имеющий дату переноса {0} уже существует", item.Date.ToString("d")));
			
			return new OperationResult();
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
	}
}