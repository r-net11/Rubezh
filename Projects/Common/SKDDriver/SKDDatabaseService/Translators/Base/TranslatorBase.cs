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
		TableT TranslateBack(ApiT apiItem)
		{
			var result = new TableT();
			result.Uid = apiItem.UID;
			Update(result, apiItem);
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
			result = result.And(e => e != null);
			
			var IsDeletedExpression = PredicateBuilder.True<TableT>();
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					IsDeletedExpression = e => e.IsDeleted;
					break;
				case DeletedType.Not:
					IsDeletedExpression = e => !e.IsDeleted;
					break;
				default:
					break;
			}
			result = result.And(IsDeletedExpression);
			
			var uids = filter.Uids;
			if (uids != null && uids.Count != 0)
				result = result.And(e => uids.Contains(e.Uid));
			var removalDates = filter.RemovalDates;
			if (removalDates != null && filter.WithDeleted == DeletedType.Deleted)
				result = result.And(e => e.RemovalDate == null || 
					(e.RemovalDate >= removalDates.StartDate && 
						e.RemovalDate <= removalDates.EndDate));
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

		public virtual OperationResult Save(IEnumerable<ApiT> items)
		{
			try
			{
				if (items == null)
					return new OperationResult();
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

		public virtual OperationResult MarkDeleted(IEnumerable<ApiT> items)
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
						{
							databaseItem.IsDeleted = true;
							databaseItem.RemovalDate = DateTime.Now;
						}
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

		public static Expression<Func<TableT, bool>> IsInDeleted(FilterT filter)
		{
			switch (filter.WithDeleted)
			{
				case DeletedType.Deleted:
					return e => e != null && e.IsDeleted;
				case DeletedType.Not:
					return e => e != null && !e.IsDeleted;
				default:
					return e => true;
			}
		}

		#endregion
	}
}