using System;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;
using System.Collections.Generic;

namespace SKDDriver.DataClasses
{
	public class HolidayTranslator : OrganisationItemTranslatorBase<Holiday, API.Holiday, API.HolidayFilter>
	{
		public HolidayTranslator(DbService context)
			: base(context)
		{
			AsyncTranslator = new HolidayAsyncTranslator(this);
		}

		public HolidayAsyncTranslator AsyncTranslator { get; private set; }

		public override DbSet<Holiday> Table
		{
			get { return Context.Holidays; }
		}

		public override void TranslateBack(API.Holiday apiItem, Holiday tableItem)
		{
			base.TranslateBack(apiItem, tableItem);
			tableItem.Date = apiItem.Date;
			tableItem.TransferDate = apiItem.TransferDate.CheckDate();
			tableItem.Type = (int)apiItem.Type;
			tableItem.Reduction = apiItem.Reduction;
		}

		public override System.Linq.IQueryable<Holiday> GetFilteredTableItems(API.HolidayFilter filter, IQueryable<Holiday> tableItems)
		{
			return base.GetFilteredTableItems(filter, tableItems).Where(x => filter.Year == 0 || x.Date.Year == filter.Year);
		}

		protected override FiresecAPI.OperationResult<bool> CanSave(API.Holiday item)
		{
			if (item == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (item.OrganisationUID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			int year = item.Date.Year;
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				x.Date.Year == year &&
				!x.IsDeleted);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			if (item.Reduction.TotalHours > 2)
				return OperationResult<bool>.FromError("Величина сокращения не может быть больше двух часов");
			if (Table.Any(x => x.UID != item.UID
				&& x.OrganisationUID == item.OrganisationUID
				&& x.Date.Year == year
				&& !x.IsDeleted
				&& x.Date == item.Date))
				return OperationResult<bool>.FromError("Дата сокращённого дня совпадает с введенной ранее");
			if (hasSameName)
				return OperationResult<bool>.FromError("Сокращённый день с таким же названием уже содержится в базе данных");
			return new OperationResult<bool>();
		}

		protected override IEnumerable<API.Holiday> GetAPIItems(IQueryable<Holiday> tableItems)
		{
			return tableItems.Select(tableItem => new API.Holiday
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate != null ? tableItem.RemovalDate.Value : new DateTime(),
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				Date = tableItem.Date,
				TransferDate = tableItem.TransferDate,
				Type = (API.HolidayType)tableItem.Type,
				Reduction = tableItem.Reduction
			});
		}
	}

	public class HolidayAsyncTranslator : AsyncTranslator<Holiday, API.Holiday, API.HolidayFilter>
	{
		public HolidayAsyncTranslator(HolidayTranslator translator) : base(translator as ITranslatorGet<Holiday, API.Holiday, API.HolidayFilter>) { }
		public override List<API.Holiday> GetCollection(DbCallbackResult callbackResult)
		{
			return callbackResult.Holidays;
		}
	}
}