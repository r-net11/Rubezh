using System;
using System.Collections.Generic;
using System.Linq;
using Common;
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

		public TranslatorBase(Table<TableT> table, DataAccess.SKUDDataContext context)
		{
			Table = table;
			Context = context;
		}

		protected virtual ApiT Translate(TableT tableItem)
		{

			var result = new ApiT();
			result.UID = tableItem.Uid;
			result.IsDeleted = tableItem.IsDeleted;
			result.RemovalDate = tableItem.RemovalDate;
			return result;
		}
		protected virtual TableT TranslateBack(ApiT apiItem)
		{
			var result = new TableT();
			result.Uid = apiItem.UID;
			result.IsDeleted = apiItem.IsDeleted;
			result.RemovalDate = CheckDate(apiItem.RemovalDate);
			return result;
		}
		protected virtual void Update(TableT tableItem, ApiT apiItem)
		{
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = CheckDate(apiItem.RemovalDate);
		}
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
			var IsDeletedExpression = PredicateBuilder.True<TableT>();
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					IsDeletedExpression = e => e != null && e.IsDeleted.GetValueOrDefault(false);
					break;
				case DeletedType.Not:
					IsDeletedExpression = e => e != null && !e.IsDeleted.GetValueOrDefault(false);
					break;
				default:
					IsDeletedExpression = e => e != null;
					break;
			}
			result = result.And(IsDeletedExpression);
			var uids = filter.Uids;
			if (uids != null && uids.Count != 0)
				result = result.And(e => uids.Contains(e.Uid));
			var removalDates = filter.RemovalDates;
			if (removalDates != null)
				result = result.And(e => e.RemovalDate == null || (e.RemovalDate >= removalDates.StartDate && e.RemovalDate <= removalDates.EndDate));
			return result;
		}

		public OperationResult<IEnumerable<ApiT>> Get(FilterT filter)
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

		public OperationResult Save(IEnumerable<ApiT> items)
		{
			try
			{
				foreach (var item in items)
				{
					if (item == null)
						continue;
					var verifyResult = CanSave(item);
					if (verifyResult.HasError)
						return verifyResult;
					var databaseItem = (from x in Table where x.Uid.Equals(item.UID) select x).FirstOrDefault();
					if (databaseItem != null)
						Update(databaseItem, item);
					else
						Table.InsertOnSubmit(TranslateBack(item));
				}
				Table.Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult MarkDeleted(IEnumerable<ApiT> items)
		{
			var operationResult = new OperationResult();
			try
			{
				foreach (var item in items)
				{
					var verifyResult = CanDelete(item);
					if (verifyResult.HasError)
						return verifyResult;
					if (item != null)
					{
						var databaseItem = (from x in Table where x.Uid.Equals(item.UID) select x).FirstOrDefault();
						if (databaseItem != null)
							databaseItem.IsDeleted = true;
					}
				}
				Table.Context.SubmitChanges();
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		#region Static
		protected static DateTime? CheckDate(DateTime? dateTime)
		{
			if (dateTime == null)
				return null;
			if (dateTime.Value.Year < 1754)
				return null;
			if (dateTime.Value.Year > 9998)
				return null;
			return dateTime;
		}

		public static Expression<Func<TableT, bool>> IsInDeleted(FilterT filter)
		{
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					return e => e != null && e.IsDeleted.GetValueOrDefault(false);
				case DeletedType.Not:
					return e => e != null && !e.IsDeleted.GetValueOrDefault(false);
				default:
					return e => true;
			}
		}

		#endregion
	}
}
