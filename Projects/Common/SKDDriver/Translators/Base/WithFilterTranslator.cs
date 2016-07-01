using StrazhAPI;
using StrazhAPI.SKD;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public abstract class WithFilterTranslator<TableT, ApiT, FilterT> : TranslatorBase<TableT, ApiT>
		where TableT : class,DataAccess.IDatabaseElement, new()
		where ApiT : SKDModelBase, new()
		where FilterT : FilterBase
	{
		protected WithFilterTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected virtual Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			result = result.And(e => e != null);
			var uids = filter.UIDs;
			if (uids != null && uids.Count != 0)
				result = result.And(e => uids.Contains(e.UID));
			var exceptUIDs = filter.ExceptUIDs;
			if (exceptUIDs != null && exceptUIDs.Count != 0)
				result = result.And(e => !exceptUIDs.Contains(e.UID));
			return result;
		}

		protected virtual IQueryable<TableT> GetQuery(FilterT filter)
		{
			var result = Table.Where(IsInFilter(filter));
			return result;
		}

		public virtual IEnumerable<TableT> GetTableItems(FilterT filter)
		{
			if (filter == null)
				return new List<TableT>();
			var query = GetQuery(filter);
			return query.ToList();
		}

		public virtual OperationResult<IEnumerable<ApiT>> Get(FilterT filter)
		{
			try
			{
				var result = new List<ApiT>();
				var tableItems = GetTableItems(filter).ToList();
				foreach (var tableItem in tableItems)
					result.Add(Translate(tableItem));
				var operationResult = new OperationResult<IEnumerable<ApiT>>(result);
				return operationResult;
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<ApiT>>.FromError(e.Message);
			}
		}
	}
}