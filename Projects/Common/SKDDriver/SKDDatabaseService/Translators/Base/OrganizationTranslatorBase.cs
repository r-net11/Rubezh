using System;
using System.Linq;
using FiresecAPI;
using System.Data.Linq;
using LinqKit;
using System.Linq.Expressions;

namespace SKDDriver
{
	public abstract class OrganizationElementTranslator<TableT, ApiT, FilterT> : IsDeletedTranslator<TableT, ApiT, FilterT>
		where TableT : class,DataAccess.IOrganizationDatabaseElement, new()
		where ApiT : OrganizationElementBase, new()
		where FilterT : OrganizationFilterBase
	{
		public OrganizationElementTranslator(DataAccess.SKUDDataContext context)
			: base(context)
		{

		}

		protected override ApiT Translate(TableT tableItem)
		{
			var result = base.Translate(tableItem);
			result.OrganizationUID = tableItem.OrganizationUID;
			return result;
		}

		protected override void TranslateBack(TableT tableItem, ApiT apiItem)
		{
			base.TranslateBack(tableItem, apiItem);
			tableItem.OrganizationUID = apiItem.OrganizationUID;
		}

		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			result = result.And(base.IsInFilter(filter));
			var organizationUIDs = filter.OrganizationUIDs;
			if (organizationUIDs != null && organizationUIDs.Count != 0)
				result = result.And(e => e.OrganizationUID != null && organizationUIDs.Contains(e.OrganizationUID.Value));
			return result;
		}
	}
}