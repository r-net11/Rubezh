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
		public WithFilterTranslator(SKDDatabaseService databaseService)
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
		protected virtual IEnumerable<TableT> GetTableItems(FilterT filter)
		{
			if (filter == null)
				return new List<TableT>();
			var query = GetQuery(filter);
			var result = new List<TableT>();
			int skipCount = 0;
			var itemsQuant = query.Skip(skipCount).Take(1000).ToList();
			skipCount += 1000;
			result.AddRange(itemsQuant);
			itemsQuant = query.Skip(skipCount).Take(1000).ToList();
			skipCount += 1000;
			result.AddRange(itemsQuant);
			while (itemsQuant.Count() == 2000)
			{
				itemsQuant = query.Skip(skipCount).Take(2000).ToList();
				skipCount += 2000;
				result.AddRange(itemsQuant);
			}
			return result;
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
