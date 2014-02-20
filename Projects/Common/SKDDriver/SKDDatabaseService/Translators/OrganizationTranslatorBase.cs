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
		public OrganizationTranslatorBase(Table<TableT> table, DataAccess.SKUDDataContext context)
			: base(table,context)
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

		protected override Expression<Func<TableT, bool>> IsInFilter(FilterT filter)
		{
			var result = PredicateBuilder.True<TableT>();
			result = result.And(base.IsInFilter(filter));
			var organizationUIDs = filter.OrganizationUids;
			if (organizationUIDs != null && organizationUIDs.Count != 0)
				result = result.And(e => organizationUIDs.Contains(e.OrganizationUid.GetValueOrDefault()));
			return result;
		}

		protected override void Update(TableT tableItem, ApiT apiItem)
		{
			base.Update(tableItem, apiItem);
			tableItem.OrganizationUid = apiItem.OrganizationUid;
		}
	}
}
