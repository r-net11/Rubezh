using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;

namespace SKDDriver
{
	public abstract class OrganizationTranslatorBase<TableT, ApiT, FilterT> : TranslatorBase<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IOrganizationDatabaseElement, new()
		where ApiT : OrganizationElementBase, new()
		where FilterT : OrganizationFilterBase
	{
		public OrganizationTranslatorBase(DataAccess.SKUDDataContext context)
			: base(context)
		{

		}

		protected override ApiT Translate(TableT tableItem)
		{
			var result = base.Translate(tableItem);
			result.OrganizationUid = tableItem.OrganizationUid;
			return result;
		}

		protected override void TranslateBack(TableT tableItem, ApiT apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.OrganizationUid = apiItem.OrganizationUid;
		}

		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			result = result.And(base.IsInFilter(filter));
			var organizationUIDs = filter.OrganizationUids;
			if (organizationUIDs != null && organizationUIDs.Count != 0)
				result = result.And(e => e.OrganizationUid != null && organizationUIDs.Contains(e.OrganizationUid.Value));
			return result;
		}
	}
}