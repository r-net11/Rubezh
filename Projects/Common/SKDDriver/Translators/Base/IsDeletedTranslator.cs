using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public abstract class IsDeletedTranslator<TableT, ApiT, FilterT> : WithFilterTranslator<TableT, ApiT, FilterT>
		where TableT : class, DataAccess.IIsDeletedDatabaseElement, DataAccess.IDatabaseElement, new()
		where ApiT : SKDIsDeletedModel, new()
		where FilterT : IsDeletedFilter
	{
		public IsDeletedTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

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

		public virtual OperationResult MarkDeleted(Guid uid)
		{
			try
			{
				var verifyResult = CanDelete(uid);
				if (verifyResult.HasError)
					return verifyResult;
				if (uid != null && uid != Guid.Empty)
				{
					var removalDate = DateTime.Now;
					var beforeDeleteResult = BeforeDelete(uid, removalDate);
					if (beforeDeleteResult.HasError)
						return beforeDeleteResult;
					var databaseItem = (from x in Table where x.UID.Equals(uid) select x).FirstOrDefault();
					if (databaseItem != null)
					{
						databaseItem.IsDeleted = true;
						databaseItem.RemovalDate = removalDate;
						Table.Context.SubmitChanges();
						return new OperationResult();
					}
					else
					{
						return new OperationResult("Не найдена запись в базе данных");
					}
				}
				else
				{
					return new OperationResult("Не задан идентификатор");
				}
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected virtual OperationResult CanDelete(Guid uid)
		{
			return new OperationResult();
		}

		protected virtual OperationResult BeforeDelete(Guid uid, DateTime removalDate)
		{
			return new OperationResult();
		}

		public virtual OperationResult Restore(Guid uid)
		{
			try
			{
				var verifyResult = CanRestore(uid);
				if (verifyResult.HasError)
					return verifyResult;
				if (uid == null || uid == Guid.Empty)
					return new OperationResult("Не задан идентификатор");
				var databaseItem = (from x in Table where x.UID.Equals(uid) select x).FirstOrDefault();
				if (databaseItem == null)
					return new OperationResult("Не найдена запись в базе данных");
				if (!databaseItem.IsDeleted)
					return new OperationResult("Данная запись не удалена");
				foreach (var item in Table)
				{
					if (!item.IsDeleted && IsSimilarNames(item, databaseItem))
						return new OperationResult("Существует неудаленная запись с тем же названием");
				}
				var beforeRestoreResult = BeforeRestore(uid, databaseItem.RemovalDate);
				if (beforeRestoreResult.HasError)
					return beforeRestoreResult;
				databaseItem.IsDeleted = false;
				Table.Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected virtual bool IsSimilarNames(TableT item1, TableT item2)
		{
			return item1.Name == item2.Name;
		}

		protected virtual OperationResult CanRestore(Guid uid)
		{
			return new OperationResult();
		}

		protected virtual OperationResult BeforeRestore(Guid uid, DateTime removalDate)
		{
			return new OperationResult();
		}

		protected static ApiType TranslateIsDeleted<ApiType, TableType>(TableType tableItem)
			where ApiType : SKDIsDeletedModel, new()
			where TableType : DataAccess.IIsDeletedDatabaseElement, DataAccess.IDatabaseElement
		{
			var result = TranslateBase<ApiType, TableType>(tableItem);
			result.IsDeleted = tableItem.IsDeleted;
			result.RemovalDate = TranslatiorHelper.CheckDate(tableItem.RemovalDate);
			return result;
		}

		protected static void TranslateBackIsDeleted<ApiType, TableType>(ApiType apiItem, TableType tableItem)
			where ApiType : SKDIsDeletedModel, new()
			where TableType : DataAccess.IIsDeletedDatabaseElement
		{
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = TranslatiorHelper.CheckDate(apiItem.RemovalDate);
		}
	}
}