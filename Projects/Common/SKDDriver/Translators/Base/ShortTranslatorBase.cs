using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
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
			try
			{
				var tableItems = GetFilteredTableItems(filter);
				var result = GetAPIItems(tableItems).ToList();
				return new OperationResult<List<TShort>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<TShort>>.FromError(e.Message);
			}
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