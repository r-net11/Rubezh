using System;
using System.Linq.Expressions;
using FiresecAPI;
using LinqKit;

namespace SKDDriver
{
	public abstract class OrganizationElementTranslator<TableT, ApiT, FilterT> : IsDeletedTranslator<TableT, ApiT, FilterT>
		where TableT : class, DataAccess.IOrganizationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement, new()
		where ApiT : OrganizationElementBase, new()
		where FilterT : OrganizationFilterBase
	{
		public OrganizationElementTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		protected override ApiT Translate(TableT tableItem)
		{
			return TranslateOrganizationElement<ApiT, TableT>(tableItem);
		}

		protected override void TranslateBack(TableT tableItem, ApiT apiItem)
		{
			TranslateBackOrganizationElement<ApiT, TableT>(apiItem, tableItem);
		}

		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			result = result.And(base.IsInFilter(filter));
			result = result.And(e => e.OrganizationUID != null && filter.OrganizationUIDs.Contains(e.OrganizationUID.Value));
			return result;
		}

		protected static ApiType TranslateOrganizationElement<ApiType, TableType>(TableType tableItem)
			where ApiType : OrganizationElementBase, new()
			where TableType : DataAccess.IOrganizationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement
		{
			var result = TranslateIsDeleted<ApiType, TableType>(tableItem);
			result.OrganizationUID = tableItem.OrganizationUID;
			return result;
		}

		protected static void TranslateBackOrganizationElement<ApiType, TableType>(ApiType apiItem, TableType tableItem)
			where ApiType : OrganizationElementBase, new()
			where TableType : DataAccess.IOrganizationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement
		{
			TranslateBackIsDeleted<ApiType, TableType>(apiItem, tableItem);
			tableItem.OrganizationUID = apiItem.OrganizationUID;
		}
	}
}