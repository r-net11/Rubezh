using System;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public class HolidayTranslator : OrganisationItemTranslatorBase<Holiday, API.Holiday, API.HolidayFilter>
	{
		public HolidayTranslator(DbService context) : base(context) { }

		public override DbSet<Holiday> Table
		{
			get { return Context.Holidays; }
		}

		public override API.Holiday Translate(Holiday tableItem)
		{
			var result = base.Translate(tableItem);
			result.Date = tableItem.Date;
			result.TransferDate = tableItem.TransferDate;
			result.Type = (API.HolidayType)tableItem.Type;
			result.Reduction = TimeSpan.FromMilliseconds(tableItem.Reduction);
			return result;
		}

		public override void TranslateBack(API.Holiday apiItem, Holiday tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Date = apiItem.Date;
			tableItem.TransferDate = apiItem.TransferDate;
			tableItem.Type = (int)apiItem.Type;
			tableItem.Reduction = (int)apiItem.Reduction.TotalMilliseconds;
		}

		public override System.Linq.IQueryable<Holiday> GetFilteredTableItems(API.HolidayFilter filter, IQueryable<Holiday> tableItems)
		{
			return base.GetFilteredTableItems(filter, tableItems).Where(x => filter.Year == 0 || x.Date.Year == filter.Year);
		}

		protected override FiresecAPI.OperationResult<bool> CanSave(API.Holiday item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			if (item.Reduction.TotalHours > 2)
				return OperationResult<bool>.FromError("Величина сокращения не может быть больше двух часов");
			int day = item.Date.Day;
			int month = item.Date.Month;
			int year = item.Date.Year;
			if (Table.Any(x => x.UID != item.UID 
				&& x.OrganisationUID == item.OrganisationUID 
				&& x.Date.Year == year
				&& !x.IsDeleted))
				return OperationResult<bool>.FromError("Дата сокращённого дня совпадает с введенной ранее");
			bool hasSameName = Table.Any(x => x.OrganisationUID == item.OrganisationUID && x.UID != item.UID && !x.IsDeleted && x.Name == item.Name 
				&& x.Date.Year == year);
			if (hasSameName)
				return OperationResult<bool>.FromError("Сокращённый день с таким же названием уже содержится в базе данных");
			return new OperationResult<bool>();
		}
	}
}
		