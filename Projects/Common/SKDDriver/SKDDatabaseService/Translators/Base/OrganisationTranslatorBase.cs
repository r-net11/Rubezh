using System;
using System.Linq;
using System.Linq.Expressions;
using FiresecAPI;
using FiresecAPI.SKD;
using LinqKit;

namespace SKDDriver
{
	public abstract class OrganisationElementTranslator<TableT, ApiT, FilterT> : IsDeletedTranslator<TableT, ApiT, FilterT>
		where TableT : class, DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement, new()
		where ApiT : OrganisationElementBase, new()
		where FilterT : OrganisationFilterBase
	{
		public OrganisationElementTranslator(DataAccess.SKDDataContext context)
			: base(context)
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

		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = base.IsInFilter(filter);
			if(filter.OrganisationUIDs.IsNotNullOrEmpty())
				result = result.And(e => e.OrganisationUID != null && filter.OrganisationUIDs.Contains(e.OrganisationUID.Value));
			if (filter.UserUID != Guid.Empty)
				result = result.And(e => (Context.Organisations.Any(x => x.OrganisationUsers.Any(y => y.UserUID == filter.UserUID))));
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
	}
}