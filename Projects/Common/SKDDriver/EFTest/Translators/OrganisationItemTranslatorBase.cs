using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
	public abstract class OrganisationItemTranslatorBase<TTableItem, TApiItem, TFilter>
		where TTableItem : class, IOrganisationItem, new()
		where TApiItem : API.IOrganisationElement, new()
		where TFilter : API.OrganisationFilterBase
	{
		protected DbService DbService; 
		protected SKDDbContext Context;

		public OrganisationItemTranslatorBase(DbService context)
		{
			DbService = context;
			Context = DbService.Context;
		}

		public OperationResult<List<TApiItem>> Get(TFilter filter)
		{
			try
			{
				var tableItems = GetFilteredTableItems(filter).ToList();
				var result = tableItems.Select(x => Translate(x)).ToList();
				return new OperationResult<List<TApiItem>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<TApiItem>>.FromError(e.Message);
			}
		}

		public OperationResult<TApiItem> GetSingle(Guid uid)
		{
			try
			{
				var tableItems = GetTableItems().FirstOrDefault(x => x.UID == uid);
				var result = Translate(tableItems);
				return new OperationResult<TApiItem>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<TApiItem>.FromError(e.Message);
			}
		}

		public virtual OperationResult<bool> Save(TApiItem item)
		{
			try
			{
				var canSaveResult = CanSave(item);
				if (canSaveResult.HasError)
					return canSaveResult;
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
				return new OperationResult<bool>(true);
			}
			catch (System.Exception e)
			{
				return OperationResult<bool>.FromError(e.Message);
			}
		}

		public OperationResult MarkDeleted(Guid uid)
		{
			try
			{
				var canDeleteResult = CanDelete(uid);
				if (canDeleteResult.HasError)
					return canDeleteResult;
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					tableItem.IsDeleted = true;
					tableItem.RemovalDate = DateTime.Now;
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult Restore(Guid uid)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					tableItem.IsDeleted = false;
					tableItem.RemovalDate = null;
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

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

		protected virtual OperationResult CanDelete(Guid uid)
		{
			return new OperationResult();
		}

		public virtual TApiItem Translate(TTableItem tableItem)
		{
			return new TApiItem
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
				OrganisationUID = tableItem.OrganisationUID.GetValueOrDefault()
			};
		}

		public virtual void TranslateBack(TApiItem apiItem, TTableItem tableItem)
		{
			tableItem.Name = apiItem.Name;
			tableItem.Description = apiItem.Description;
			tableItem.IsDeleted = apiItem.IsDeleted;
			tableItem.RemovalDate = apiItem.RemovalDate;
			tableItem.OrganisationUID = apiItem.OrganisationUID;
		}
		protected abstract DbSet<TTableItem> Table { get; }
		protected virtual IQueryable<TTableItem> GetTableItems()
		{
			return Table;//.Include(x => x.Organisation.OrganisationUsers);
		}
		protected virtual void ClearDependentData(TTableItem tableItem) { }
		protected virtual IQueryable<TTableItem> GetFilteredTableItems(TFilter filter)
		{
			return GetTableItems().Where(x =>
				(filter.UIDs.Count() == 0 || filter.UIDs.Contains(x.UID)) &&
				(filter.ExceptUIDs.Count() == 0 || !filter.ExceptUIDs.Contains(x.UID)) &&
				(filter.LogicalDeletationType != API.LogicalDeletationType.Active || !x.IsDeleted) &&
				(filter.LogicalDeletationType != API.LogicalDeletationType.Deleted || x.IsDeleted) //&&
				//(filter.OrganisationUIDs.Count() == 0 ||
				//    (x.OrganisationUID != null && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value))) &&
				//(filter.UserUID != Guid.Empty ||
				//    x.Organisation != null && x.Organisation.OrganisationUsers.Any(organisationUser => organisationUser.UserUID == filter.UserUID)) 
			);
		}
	}
}
