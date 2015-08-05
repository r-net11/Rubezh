using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public interface ITranslatorGet<TTableItem, TApiItem, TFilter>
		where TTableItem : class
	{
		IQueryable<TTableItem> GetFilteredTableItems(TFilter filter, IQueryable<TTableItem> tableItems);
		IQueryable<TTableItem> GetTableItems();
		TApiItem Translate(TTableItem tableItem);
		DbService DbService { get; }
		DbSet<TTableItem> Table { get; }
	}
}