using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public abstract class ShortTranslatorBase<TTableItem, TShort, TApiItem, TFilter> : ITranslatorGet<TTableItem, TShort, TFilter>
		where TTableItem : class, new()
		where TApiItem : class, new()
		where TShort : class, new()
	{
		protected ITranslatorGet<TTableItem, TApiItem, TFilter> ParentTranslator;
		public DbService DbService { get { return ParentTranslator.DbService; } }
		DatabaseContext Context { get { return DbService.Context; } }

		public ShortTranslatorBase(ITranslatorGet<TTableItem, TApiItem, TFilter> tranlsator)
		{
			ParentTranslator = tranlsator;
		}

		public virtual IQueryable<TTableItem> GetTableItems()
		{
			return Table;
		}

		public virtual OperationResult<List<TShort>> Get(TFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetFilteredTableItems(filter);
				return GetAPIItems(tableItems).ToList();
			});
		}

		protected abstract IEnumerable<TShort> GetAPIItems(IQueryable<TTableItem> tableItems);
		
		public IQueryable<TTableItem> GetFilteredTableItems(TFilter filter)
		{
			return GetFilteredTableItems(filter, GetTableItems());
		}

		public IQueryable<TTableItem> GetFilteredTableItems(TFilter filter, IQueryable<TTableItem> tableItems)
		{
			return ParentTranslator.GetFilteredTableItems(filter, tableItems);
		}

		public DbSet<TTableItem> Table
		{
			get { return ParentTranslator.Table; }
		}
	}
}