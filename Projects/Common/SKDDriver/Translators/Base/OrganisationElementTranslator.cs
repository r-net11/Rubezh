using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace StrazhDAL
{
	public abstract class OrganisationElementTranslator<TableT, ApiT, FilterT> : IsDeletedTranslator<TableT, ApiT, FilterT>
		where TableT : class, DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement, new()
		where ApiT : OrganisationElementBase, new()
		where FilterT : OrganisationFilterBase
	{
		public OrganisationElementTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		protected override ApiT Translate(TableT tableItem)
		{
			return TranslateOrganisationElement<ApiT, TableT>(tableItem);
		}

		protected override void TranslateBack(TableT tableItem, ApiT apiItem)
		{
			TranslateBackOrganisationElement<ApiT, TableT>(apiItem, tableItem);
		}

		public virtual OperationResult MarkDeletedByOrganisation(Guid organisationUID, DateTime removalDate)
		{
			try
			{
				var databaseItems = Table.ToList().Where(x => x.OrganisationUID == organisationUID && !x.IsDeleted);
				if (databaseItems != null && databaseItems.Count() > 0)
				{
					foreach (var item in databaseItems)
					{
						item.IsDeleted = true;
						item.RemovalDate = removalDate;
					}
					Table.Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		public OperationResult RestoreByOrganisation(Guid organisationUID, DateTime removalDate)
		{
			try
			{
				var databaseItems = Table.ToList().Where(x => x.OrganisationUID == organisationUID && x.IsDeleted && x.RemovalDate == removalDate);
				if (databaseItems != null && databaseItems.Count() > 0)
				{
					foreach (var item in databaseItems)
						item.IsDeleted = false;
					Table.Context.SubmitChanges();
				}
				return new OperationResult();
			}
			catch (Exception e)
			{
				return new OperationResult(e.Message);
			}
		}

		protected override OperationResult CanSave(ApiT item)
		{
			var result = base.CanSave(item);
			if (result.HasError)
				return result;
			if (item.OrganisationUID == Guid.Empty)
				return new OperationResult("Не указана организация");
			else
				return new OperationResult();
		}

		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = base.IsInFilter(filter);
			if (filter.OrganisationUIDs.IsNotNullOrEmpty())
				result = result.And(x => x.OrganisationUID != null && filter.OrganisationUIDs.Contains(x.OrganisationUID.Value));
			if (filter.UserUID != Guid.Empty)
				result = result.And(e => Context.Organisations.Any(x => x.OrganisationUsers.Any(y => y.UserUID == filter.UserUID) && x.UID == e.OrganisationUID));
			return result;
		}

		protected static ApiType TranslateOrganisationElement<ApiType, TableType>(TableType tableItem)
			where ApiType : OrganisationElementBase, new()
			where TableType : DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement
		{
			var result = TranslateIsDeleted<ApiType, TableType>(tableItem);
			result.OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : result.OrganisationUID = Guid.Empty;
			return result;
		}

		protected static void TranslateBackOrganisationElement<ApiType, TableType>(ApiType apiItem, TableType tableItem)
			where ApiType : OrganisationElementBase, new()
			where TableType : DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement
		{
			TranslateBackIsDeleted<ApiType, TableType>(apiItem, tableItem);
			tableItem.OrganisationUID = apiItem.OrganisationUID;
		}

		protected override bool IsSimilarNames(TableT item1, TableT item2)
		{
			return base.IsSimilarNames(item1, item2) && item1.OrganisationUID == item2.OrganisationUID;
		}
	}
}