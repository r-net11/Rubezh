using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public abstract class IsDeletedTranslator<TableT, ApiT, FilterT> : TranslatorBase<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IIsDeletedDatabaseElement, DataAccess.IDatabaseElement, new()
		where ApiT : SKDIsDeletedModel, new()
		where FilterT : IsDeletedFilter
	{
		public IsDeletedTranslator(DataAccess.SKDDataContext context) : base(context) { }

		protected override ApiT Translate(TableT tableItem)
		{
			return TranslateIsDeleted<ApiT, TableT>(tableItem);
		}
		protected override void TranslateBack(TableT tableItem, ApiT apiItem)
		{
			TranslateBackIsDeleted<ApiT, TableT>(apiItem, tableItem);
		}
		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = base.IsInFilter(filter);
			var IsDeletedExpression = PredicateBuilder.True<TableT>();
			switch (filter.LogicalDeletationType)
			{
				case LogicalDeletationType.Deleted:
					IsDeletedExpression = e => e.IsDeleted;
					break;
				case LogicalDeletationType.Active:
					IsDeletedExpression = e => !e.IsDeleted;
					break;
				default:
					break;
			}
			result = result.And(IsDeletedExpression);
			return result;
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
						var databaseItem = (from x in Table where x.UID.Equals(item.UID) select x).FirstOrDefault();
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

		public virtual OperationResult MarkDeleted(Guid uid)
		{
			var operationResult = new OperationResult();
			try
			{
				var verifyResult = CanDelete(uid);
				if (verifyResult.HasError)
					return verifyResult;
				if (uid != null)
				{
					var databaseItem = (from x in Table where x.UID.Equals(uid) select x).FirstOrDefault();
					if (databaseItem != null)
					{
						databaseItem.IsDeleted = true;
						databaseItem.RemovalDate = DateTime.Now;
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

		public virtual OperationResult MarkDeleted(IEnumerable<Guid> UIDs)
		{
			var operationResult = new OperationResult();
			try
			{
				foreach (var uid in UIDs)
				{
					if (uid != null)
					{
						var databaseItem = (from x in Table where x.UID.Equals(uid) select x).FirstOrDefault();
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

		public static Expression<Func<TableT, bool>> IsInDeleted(FilterT filter)
		{
			switch (filter.LogicalDeletationType)
			{
				case LogicalDeletationType.Deleted:
					return e => e != null && e.IsDeleted;
				case LogicalDeletationType.Active:
					return e => e != null && !e.IsDeleted;
				default:
					return e => true;
			}
		}

		protected static ApiType TranslateIsDeleted<ApiType, TableType>(TableType tableItem)
			where ApiType : SKDIsDeletedModel, new()
			where TableType : DataAccess.IIsDeletedDatabaseElement, DataAccess.IDatabaseElement
		{
			var result = TranslateBase<ApiType, TableType>(tableItem);
			result.IsDeleted = tableItem.IsDeleted;
			result.RemovalDate = CheckDate(tableItem.RemovalDate);
			return result;
		}

		protected static void TranslateBackIsDeleted<ApiType, TableType>(ApiType apiItem, TableType tableItem)
			where ApiType : SKDIsDeletedModel, new()
			where TableType : DataAccess.IIsDeletedDatabaseElement
		{
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = CheckDate(apiItem.RemovalDate);
		}
	}
}