using FiresecAPI;
using FiresecAPI.SKD;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace SKDDriver
{
	public abstract class TranslatorBase<TableT, ApiT>
		where TableT : class,DataAccess.IDatabaseElement, new()
		where ApiT : SKDModelBase, new()
	{
		protected Table<TableT> Table;
		protected SKDDatabaseService DatabaseService;
		protected DataAccess.SKDDataContext Context;

		public TranslatorBase(SKDDatabaseService databaseService)
		{
			DatabaseService = databaseService;
			Context = databaseService.Context;
			Table = Context.GetTable<TableT>();
		}

		protected virtual ApiT Translate(TableT tableItem)
		{
			return TranslateBase<ApiT, TableT>(tableItem);
		}

		protected abstract void TranslateBack(TableT tableItem, ApiT apiItem);

		protected virtual OperationResult CanSave(ApiT item)
		{
			if (item == null)
                return new OperationResult(Resources.Language.Translators.Base.TranslatorBase.SaveEmptyRecord);
			return new OperationResult();
		}

		public virtual OperationResult Save(IEnumerable<ApiT> apiItems)
		{
			if (apiItems == null || apiItems.Count() == 0)
				return new OperationResult();
			foreach (var apiItem in apiItems)
			{
				var verifyResult = CanSave(apiItem);
				if (verifyResult.HasError)
					return verifyResult;
			}
			try
			{
				foreach (var apiItem in apiItems)
					OnSave(apiItem);
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
			var verifyResult = CanSave(apiItem);
			if (verifyResult.HasError)
				return verifyResult;
			try
			{
				OnSave(apiItem);
				Context.SubmitChanges();
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		private void OnSave(ApiT apiItem)
		{
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

		protected virtual int Comparer(TableT item1, TableT item2)
		{
			return 0;
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
			return GetAllByEmployee<T>(uid, Context.GetTable<T>());
		}

		public List<ApiT> GetAllByEmployee<T>(Guid uid, IEnumerable<T> tableItems)
			where T : class, DataAccess.ILinkedToEmployee, TableT
		{
			var result = new List<ApiT>();
			var table = Context.GetTable<T>();
			foreach (var tableItem in tableItems.Where(x => x.EmployeeUID.Equals(uid)))
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
				if (uid == null || uid == Guid.Empty)
					return new OperationResult<ApiT>(null);
				var tableItem = Table.Where(x => x.UID.Equals(uid.Value)).FirstOrDefault();
				if (tableItem == null)
					return new OperationResult<ApiT>(null);
				var result = Translate(tableItem);
				return new OperationResult<ApiT>(result);
			}
			catch (Exception e)
			{
				return OperationResult<ApiT>.FromError(e.Message);
			}
		}

		public virtual OperationResult Delete(Guid uid)
		{
			try
			{
				var tableItem = Table.Where(x => x.UID.Equals(uid)).FirstOrDefault();
				if (tableItem != null)
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

	public static class TranslatiorHelper
	{
		public static readonly DateTime MinYear = new DateTime(1900, 1, 1);
		public static readonly DateTime MaxYear = new DateTime(9000, 1, 1);

		public static DateTime CheckDate(DateTime dateTime)
		{
			if (dateTime < MinYear)
				return MinYear;
			if (dateTime > MaxYear)
				return MaxYear;
			return dateTime;
		}

		public static OperationResult ConcatOperationResults(params OperationResult[] results)
		{
			var result = new OperationResult();
			foreach (var item in results)
			{
				if (item.HasError)
				{
					result.HasError = true;
					result.Error = string.Format("{0} {1}", result.Error, item.Error);
				}
			}
			return result;
		}
	}
}