using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDDriver
{
	public abstract class TranslatorBase<TableT, ApiT>
		where TableT : class,DataAccess.IDatabaseElement, new()
		where ApiT : SKDModelBase, new()
		
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
		protected virtual OperationResult CanDelete(Guid uid)
		{
			return new OperationResult();
		}

		public virtual OperationResult Save(IEnumerable<ApiT> apiItems)
		{
			try
			{
				if (apiItems == null || apiItems.Count() == 0)
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

		public virtual OperationResult Save(ApiT apiItem)
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
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
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
				if (uid == null || uid == Guid.Empty)
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

		public OperationResult Delete(Guid uid)
		{
			try
			{
				var tableItem = Table.Where(x => x.UID.Equals(uid)).Single();
				Table.DeleteOnSubmit(tableItem);
				Table.Context.SubmitChanges();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
			return new OperationResult();
		}
	}	
}