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

        public virtual TShort Translate(TTableItem tableItem)
        {
            if (tableItem == null)
                return null;
            return new TShort();
        }

        public virtual IQueryable<TTableItem> GetTableItems()
        {
            return Table;
        }

        public OperationResult<List<TShort>> Get(TFilter filter)
        {
            try
            {
                var tableItems = GetFilteredTableItems(filter).ToList();
                var result = tableItems.Select(x => Translate(x)).ToList();
                return new OperationResult<List<TShort>>(result);
            }
            catch (System.Exception e)
            {
                return OperationResult<List<TShort>>.FromError(e.Message);
            }
        }

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
