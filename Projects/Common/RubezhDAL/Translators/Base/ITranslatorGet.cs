using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public interface ITranslatorGet<TTableItem, TApiItem, TFilter>
		where TTableItem : class
	{
		IQueryable<TTableItem> GetFilteredTableItems(TFilter filter, IQueryable<TTableItem> tableItems);
		IQueryable<TTableItem> GetTableItems();
		DbService DbService { get; }
		DbSet<TTableItem> Table { get; }
	}
}