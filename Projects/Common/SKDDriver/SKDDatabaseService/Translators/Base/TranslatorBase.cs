using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public abstract class TranslatorBase<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IDatabaseElement, new()
		where ApiT : SKDModelBase, new()
		where FilterT : FilterBase
	{
		protected Table<TableT> Table;
		protected DataAccess.SKDDataContext Context;

		public TranslatorBase(DataAccess.SKDDataContext context)
		{
			Context = context;
			Table = Context.GetTable<TableT>();
		}

		protected virtual ApiT Translate(TableT tableItem)
		{
			return TranslateBase<ApiT, TableT>(tableItem);
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
		protected virtual OperationResult CanDelete(Guid uid)
		{
			return new OperationResult();
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
			return Table.Where(IsInFilter(filter));
		}
		protected virtual IEnumerable<TableT> GetTableItems(FilterT filter)
		{
			var query = GetQuery(filter);
			return query.ToList();
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

		public virtual OperationResult Save(IEnumerable<ApiT> apiItems)
		{
			return Save(apiItems, true);
		}
		public virtual OperationResult Save(ApiT apiItem)
		{
			return Save(apiItem, true);
		}
		public virtual OperationResult Save(IEnumerable<ApiT> apiItems, bool commit)
		{
			try
			{
				if (apiItems == null)
					return new OperationResult();
				var tableItems = new List<TableT>();
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
					tableItems.Add(tableItem);
				}
				BeforeSave(apiItems, tableItems);
				if (commit)
					Table.Context.SubmitChanges();
				AfterSave(apiItems, tableItems);
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public virtual OperationResult Save(ApiT apiItem, bool commit)
		{
			try
			{
				if (apiItem == null)
					return new OperationResult("Попытка сохранить пустую запись");
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
				if (commit)
					Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected virtual void BeforeSave(IEnumerable<ApiT> apiItems, IEnumerable<TableT> tableItems)
		{
		}
		protected virtual void AfterSave(IEnumerable<ApiT> apiItems, IEnumerable<TableT> tableItems)
		{
		}

		protected virtual int Comparer(TableT item1, TableT item2)
		{
			return 0;
		}

		protected static readonly DateTime MinYear = new DateTime(1900, 1, 1);
		protected static readonly DateTime MaxYear = new DateTime(9000, 1, 1);

		protected static DateTime CheckDate(DateTime dateTime)
		{
			if (dateTime < MinYear)
				return MinYear;
			if (dateTime > MaxYear)
				return MaxYear;
			return dateTime;
		}

		protected static ApiType TranslateBase<ApiType, TableType>(TableType tableItem)
			where ApiType : SKDModelBase, new()
			where TableType : DataAccess.IDatabaseElement
		{
			var result = new ApiType();
			result.UID = tableItem.UID;
			return result;
		}

		protected static T GetResult<T>(OperationResult<T> operationResult)
		{
			if (operationResult.HasError)
				throw new Exception(operationResult.Error);
			return operationResult.Result;
		}

		public List<ApiT> GetAllByEmployee<T>(Guid uid)
			where T : class, DataAccess.ILinkedToEmployee, TableT
		{
			var result = new List<ApiT>();
			var table = Context.GetTable<T>();
			foreach (var tableItem in table.Where(x => x.EmployeeUID.Equals(uid)))
				result.Add(Translate(tableItem));
			return result;
		}

		public List<ApiT> GetByEmployee<T>(Guid uid)
			where T : class, DataAccess.ILinkedToEmployee, DataAccess.IIsDeletedDatabaseElement, TableT
		{
			var result = new List<ApiT>();
			var table = Context.GetTable<T>();
			foreach (var tableItem in table.Where(x => !x.IsDeleted && x.EmployeeUID.Equals(uid)))
				result.Add(Translate(tableItem));
			return result;
		}

		public OperationResult<ApiT> GetSingle(Guid? uid)
		{
			try
			{
				var result = new OperationResult<ApiT>();
				if (uid == null)
					return result;
				var tableItem = Table.Where(x => x.UID.Equals(uid.Value)).FirstOrDefault();
				if (tableItem == null)
					return result;
				result.Result = Translate(tableItem);
				return result;
			}
			catch (Exception e)
			{
				return new OperationResult<ApiT>(e.Message);
			}
		}
	}
}