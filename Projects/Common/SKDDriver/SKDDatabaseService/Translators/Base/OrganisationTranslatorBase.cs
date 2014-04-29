using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FiresecAPI;
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
			var result = PredicateBuilder.True<TableT>();
			result = result.And(base.IsInFilter(filter));
			result = result.And(e => e.OrganisationUID != null && filter.OrganisationUIDs.Contains(e.OrganisationUID.Value));
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

	public abstract class WithShortTranslator<TableT, ApiT, FilterT, ShortT> : OrganisationElementTranslator<TableT, ApiT, FilterT>
		where TableT : class, DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement, new()
		where ApiT : OrganisationElementBase, new()
		where FilterT : OrganisationFilterBase
		where ShortT: new()
	{
		public WithShortTranslator(DataAccess.SKDDataContext context)
			: base(context)
		{

		}

		public virtual OperationResult<IEnumerable<ShortT>> GetList(FilterT filter)
		{
			try
			{
				var result = new List<ShortT>();
				foreach (var tableItem in GetTableItems(filter))
				{
					var employeeListItem = TranslateToShort(tableItem);
					result.Add(employeeListItem);
				}
				var operationResult = new OperationResult<IEnumerable<ShortT>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<ShortT>>(e.Message);
			}
		}

		protected abstract ShortT TranslateToShort(TableT tableItem);
	}
}