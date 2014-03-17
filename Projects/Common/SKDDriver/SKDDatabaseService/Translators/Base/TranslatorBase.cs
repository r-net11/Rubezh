using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;

namespace SKDDriver
{
	public abstract class TranslatorBase<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IDatabaseElement, new()
		where ApiT : SKDModelBase, new()
		where FilterT : FilterBase
	{
		protected Table<TableT> Table;
		protected DataAccess.SKUDDataContext Context;

		public TranslatorBase(DataAccess.SKUDDataContext context)
		{
			Context = context;
			Table = Context.GetTable<TableT>();
		}

		protected virtual ApiT Translate(TableT tableItem)
		{
			var result = new ApiT();
			result.UID = tableItem.UID;
			return result;
		}
		protected abstract void TranslateBack(TableT tableItem, ApiT apiItem);
		
		protected virtual OperationResult CanSave(ApiT item)
		{
			return new OperationResult();
		}
		protected virtual OperationResult CanDelete(ApiT item)
		{
			return new OperationResult();
		}
		protected virtual Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			result = result.And(e => e != null);

			var uids = filter.Uids;
			if (uids != null && uids.Count != 0)
				result = result.And(e => uids.Contains(e.UID));
			return result;
		}

		public virtual OperationResult<IEnumerable<ApiT>> Get(FilterT filter)
		{
			try
			{
				List<ApiT> result = new List<ApiT>();
				IQueryable<TableT> query;
				if (filter == null)
					query = Table;
				else
					query = Table.Where(IsInFilter(filter));
				foreach (var item in query)
					result.Add(Translate(item));
				var operationResult = new OperationResult<IEnumerable<ApiT>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<ApiT>>(e.Message);
			}
		}

		public virtual OperationResult Save(IEnumerable<ApiT> apiItems)
		{
			try
			{
				if (apiItems == null)
					return new OperationResult();
				foreach (var apiItem in apiItems)
				{
					if (apiItem == null)
						continue;
					var verifyResult = CanSave(apiItem);
					if (verifyResult.HasError)
						return verifyResult;
					var tableItem = (from x in Table where x.UID.Equals(apiItem.UID) select x).FirstOrDefault();
					if (tableItem == null)
					{
						tableItem = new TableT();
						tableItem.UID = apiItem.UID;
						TranslateBack(tableItem, apiItem);
						Table.InsertOnSubmit(tableItem);
					}
					else
						TranslateBack(tableItem, apiItem);
				}
				Table.Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		static readonly DateTime MinYear = new DateTime(1900, 1, 1);
		static readonly DateTime MaxYear = new DateTime(9000, 1, 1);

		protected static DateTime CheckDate(DateTime dateTime)
		{
			if (dateTime < MinYear)
				return MinYear;
			if (dateTime > MaxYear)
				return MaxYear;
			return dateTime;
		}
	}
}