using System;
using FiresecAPI;
using System.Data.Linq;

namespace SKDDriver
{
	public abstract class OrganizationTranslatorBase<TableT, ApiT, FilterT> : TranslatorBase<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IOrganizationDatabaseElement, new()
		where ApiT : OrganizationElementBase, new()
		where FilterT : OrganizationFilterBase
	{
		public OrganizationTranslatorBase(Table<TableT> table)
			: base(table)
		{

		}

		protected override ApiT Translate(TableT tableItem)
		{
			var result = base.Translate(tableItem);
			result.OrganizationUid = tableItem.OrganizationUid;
			return result;
		}

		protected override TableT TranslateBack(ApiT apiItem)
		{
			var result = base.TranslateBack(apiItem);
			result.OrganizationUid = apiItem.OrganizationUid;
			return result;
		}

		protected override bool IsInFilter(TableT tableItem, FilterT filter)
		{
			bool isInOrganizations = IsInList<Guid>(tableItem.OrganizationUid, filter.OrganizationUids);
			bool isInBase = base.IsInFilter(tableItem, filter);
			return isInBase && isInOrganizations;
		}

		protected override void Update(TableT tableItem, ApiT apiItem)
		{
			base.Update(tableItem, apiItem);
			tableItem.OrganizationUid = apiItem.OrganizationUid;
		}
	}
}
