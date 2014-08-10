using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI.EmployeeTimeIntervals;
using LinqKit;
using OperationResult = FiresecAPI.OperationResult;

namespace SKDDriver.Translators
{
	public class HolidayTranslator : OrganisationElementTranslator<DataAccess.Holiday, Holiday, HolidayFilter>
	{
		public HolidayTranslator(DataAccess.SKDDataContext context)
			: base(context)
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
			if (item.Reduction.TotalHours > 2)
				return new OperationResult("Величина сокращения не может быть больше двух часов");
			//if (item.Type == HolidayType.WorkingHoliday && item.Date.DayOfWeek != DayOfWeek.Saturday && item.Date.DayOfWeek != DayOfWeek.Sunday)
			//    return new OperationResult("Дата переноса устанавливается только на субботу или воскресенье");
			if (Table.Any(x => x.UID != item.UID && x.OrganisationUID == item.OrganisationUID && x.Date.Date == item.Date.Date))
				return new OperationResult("Дата праздника совпадает с введенным ранее");
			return base.CanSave(item);
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
			tableItem.TransferDate = apiItem.TransferDate < MinYear ? MinYear : apiItem.TransferDate;
			tableItem.Type = (int)apiItem.Type;
		}
	}
}