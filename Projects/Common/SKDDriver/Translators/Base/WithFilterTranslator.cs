using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
    public abstract class WithFilterTranslator<TableT, ApiT, FilterT> : TranslatorBase<TableT, ApiT>
        where TableT : class,DataAccess.IDatabaseElement, new()
        where ApiT : SKDModelBase, new()
        where FilterT : FilterBase
    {
        public WithFilterTranslator(DataAccess.SKDDataContext context)
            : base(context)
        {

        }

        protected virtual Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
        {
            var result = PredicateBuilder.True<TableT>();
            result = result.And(e => e != null);
            var uids = filter.UIDs;
            if (uids != null && uids.Count != 0)
                result = result.And(e => uids.Contains(e.UID));
            return result;
        }

        protected virtual IQueryable<TableT> GetQuery(FilterT filter)
        {
            var result = Table.Where(IsInFilter(filter));
            return result;
        }
        protected virtual IEnumerable<TableT> GetTableItems(FilterT filter)
        {
            if (filter == null)
                return new List<TableT>();
            return GetQuery(filter).ToList();
        }

        public virtual OperationResult<IEnumerable<ApiT>> Get(FilterT filter)
        {
            try
            {
                var result = new List<ApiT>();
                foreach (var tableItem in GetTableItems(filter))
                    result.Add(Translate(tableItem));
                var operationResult = new OperationResult<IEnumerable<ApiT>>();
                operationResult.Result = result;
                return operationResult;
            }
            catch (Exception e)
            {
                return new OperationResult<IEnumerable<ApiT>>(e.Message);
            }
        }
    }
}
