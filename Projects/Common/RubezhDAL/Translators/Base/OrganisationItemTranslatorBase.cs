using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public abstract class OrganisationItemTranslatorBase<TTableItem, TApiItem, TFilter> : ITranslatorGet<TTableItem, TApiItem, TFilter>
		where TTableItem : class, IOrganisationItem, new()
		where TApiItem : class, API.IOrganisationElement, new()
		where TFilter : API.OrganisationFilterBase
	{
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } }

		public OrganisationItemTranslatorBase(DbService context)
		{
			DbService = context;
		}

		public OperationResult<List<TApiItem>> Get(TFilter filter)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItems = GetFilteredTableItems(filter, GetTableItems());
				return GetAPIItems(tableItems).ToList();
			});
		}

		public OperationResult<TApiItem> GetSingle(Guid? uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				if (uid == null)
					return null;
				var tableItems = GetTableItems().Where(x => x.UID == uid.Value);
				return GetAPIItems(tableItems).FirstOrDefault();
			});
		}

		public virtual OperationResult<bool> Save(TApiItem item)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var canSaveResult = CanSave(item);
				if (canSaveResult.HasError)
					throw new Exception(canSaveResult.Error);
				var tableItem = GetTableItems().FirstOrDefault(x => x.UID == item.UID);
				if (tableItem == null)
				{
					tableItem = new TTableItem { UID = item.UID };
					TranslateBack(item, tableItem);
					Table.Add(tableItem);
				}
				else
				{
					ClearDependentData(tableItem);
					TranslateBack(item, tableItem);
				}
				Context.SaveChanges();
				return true;
			});
		}

		public OperationResult<bool> MarkDeleted(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var canDeleteResult = CanDelete(uid);
				if (canDeleteResult.HasError)
					throw new Exception(canDeleteResult.Error);
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					tableItem.IsDeleted = true;
					tableItem.RemovalDate = DateTime.Now;
					AfterDelete(tableItem);
					Context.SaveChanges();
				}
				return true;
			});
		}

		protected virtual void AfterDelete(TTableItem tableItem) { }

		public OperationResult<bool> Restore(Guid uid)
		{
			return DbServiceHelper.InTryCatch(() =>
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					BeforeRestore(tableItem);
					tableItem.IsDeleted = false;
					tableItem.RemovalDate = null;
					Context.SaveChanges();
				}
				return true;
			});
		}

		protected virtual void BeforeRestore(TTableItem tableItem) { }

		protected virtual OperationResult<bool> CanSave(TApiItem item)
		{
			if (item == null)
				return OperationResult<bool>.FromError("Попытка сохранить пустую запись");
			if (item.OrganisationUID == Guid.Empty)
				return OperationResult<bool>.FromError("Не указана организация");
			bool hasSameName = Table.Any(x => x.Name == item.Name &&
				x.OrganisationUID == item.OrganisationUID &&
				x.UID != item.UID &&
				!x.IsDeleted);
			if (hasSameName)
				return OperationResult<bool>.FromError("Запись с таким же названием уже существует");
			else
				return new OperationResult<bool>(true);
		}

		protected virtual OperationResult<bool> CanDelete(Guid uid)
		{
			return new OperationResult<bool>(true);
		}

		protected abstract IEnumerable<TApiItem> GetAPIItems(IQueryable<TTableItem> tableItems);
		
		//public virtual TApiItem Translate(TTableItem tableItem)
		//{
		//	if (tableItem == null)
		//		return null;
		//	return new TApiItem
		//	{
		//		UID = tableItem.UID,
		//		Name = tableItem.Name,
		//		Description = tableItem.Description,
		//		IsDeleted = tableItem.IsDeleted,
		//		RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
		//		OrganisationUID = tableItem.OrganisationUID.GetValueOrDefault()
		//	};
		//}

		public virtual void TranslateBack(TApiItem apiItem, TTableItem tableItem)
		{
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = apiItem.RemovalDate.CheckDate();
			tableItem.OrganisationUID = apiItem.OrganisationUID;
		}
		public abstract DbSet<TTableItem> Table { get; }
		public virtual IQueryable<TTableItem> GetTableItems()
		{
			return Table.Include(x => x.Organisation.Users);
		}
		protected virtual void ClearDependentData(TTableItem tableItem) { }
		public virtual IQueryable<TTableItem> GetFilteredTableItems(TFilter filter, IQueryable<TTableItem> tableItems)
		{
			var result = tableItems;
			if(filter.UIDs.Count() > 0)
				result = result.Where(x => filter.UIDs.Contains(x.UID));
			if(filter.ExceptUIDs.Count() > 0)
				result = result.Where(x => !filter.UIDs.Contains(x.UID));
			if(filter.LogicalDeletationType == API.LogicalDeletationType.Active)
				result = result.Where(x => !x.IsDeleted);
			if(filter.LogicalDeletationType == API.LogicalDeletationType.Deleted)
				result = result.Where(x => x.IsDeleted);
			if(filter.OrganisationUIDs.Count() > 0)
				result = result.Where(x => filter.OrganisationUIDs.Contains(x.OrganisationUID.Value));
			if(filter.User != null && filter.User.UID != Guid.Empty && !filter.User.IsAdm)
				result = result.Where(x => x.Organisation != null && x.Organisation.Users.Any(organisationUser => organisationUser.UserUID == filter.User.UID));
			return result;
		}

		public IQueryable<TTableItem> GetFilteredTableItems(TFilter filter)
		{
			return GetFilteredTableItems(filter, GetTableItems());
		}
	}
}