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
		public DbService DbService { get; private set; }
		public DatabaseContext Context { get { return DbService.Context; } } 

		public OrganisationItemTranslatorBase(DbService context)
		{
			DbService = context;
		}

		public OperationResult<List<TApiItem>> Get(TFilter filter)
		{
			try
			{
				var tableItems = GetFilteredTableItems(filter, GetTableItems()).ToList();
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
					AfterDelete(tableItem);
					Context.SaveChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected virtual void AfterDelete(TTableItem tableItem) { }

		public OperationResult Restore(Guid uid)
		{
			try
			{
				var tableItem = Table.FirstOrDefault(x => x.UID == uid);
				if (tableItem != null)
				{
					BeforeRestore(tableItem);
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
		public abstract DbSet<TTableItem> Table { get; }
		protected virtual IQueryable<TTableItem> GetTableItems()
		{
			return Table.Include(x => x.Organisation.Users);
		}
		protected virtual void ClearDependentData(TTableItem tableItem) { }
		public virtual IQueryable<TTableItem> GetFilteredTableItems(TFilter filter, IQueryable<TTableItem> tableItems)
		{
			return tableItems.Where(x =>
				(filter.UIDs.Count() == 0 || filter.UIDs.Contains(x.UID)) &&
				(filter.ExceptUIDs.Count() == 0 || !filter.ExceptUIDs.Contains(x.UID)) &&
				(filter.LogicalDeletationType != API.LogicalDeletationType.Active || !x.IsDeleted) &&
				(filter.OrganisationUIDs.Count() == 0 ||
					(x.OrganisationUID != null && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value))) &&
				(filter.UserUID == Guid.Empty ||
					x.Organisation != null && x.Organisation.Users.Any(organisationUser => organisationUser.UserUID == filter.UserUID)) 
			);
		}
	}

	public abstract class ShortTranslatorBase<TTableItem, TShort, TApiItem, TFilter>
		where TTableItem : class, IOrganisationItem, new()
		where TApiItem : API.IOrganisationElement, new()
		where TShort : API.IOrganisationElement, new()
		where TFilter : API.OrganisationFilterBase
	{
		OrganisationItemTranslatorBase<TTableItem, TApiItem, TFilter> OrganisationItemTranslatorBase;
		DbService DbService { get { return OrganisationItemTranslatorBase.DbService; } }
		DatabaseContext Context { get { return DbService.Context; } }

		public ShortTranslatorBase(OrganisationItemTranslatorBase<TTableItem, TApiItem, TFilter> organisationItemTranslatorBase)
		{
			OrganisationItemTranslatorBase = organisationItemTranslatorBase;
		}

		public virtual TShort TranslateToShort(TTableItem tableItem)
		{
			return new TShort
			{
				UID = tableItem.UID,
				Name = tableItem.Name,
				Description = tableItem.Description,
				IsDeleted = tableItem.IsDeleted,
				RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
				OrganisationUID = tableItem.OrganisationUID.GetValueOrDefault()
			};		
		}

		public virtual IQueryable<TTableItem> GetTableItems()
		{
			return OrganisationItemTranslatorBase.Table.Include(x => x.Organisation.Users);
		}

		public OperationResult<List<TShort>> Get(TFilter filter)
		{
			try
			{
				var tableItems = OrganisationItemTranslatorBase.GetFilteredTableItems(filter, GetTableItems()).ToList();
				var result = tableItems.Select(x => TranslateToShort(x)).ToList();
				return new OperationResult<List<TShort>>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<List<TShort>>.FromError(e.Message);
			}
		}
	}
}
