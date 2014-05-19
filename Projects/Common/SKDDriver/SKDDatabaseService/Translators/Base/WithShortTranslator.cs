using System;
using System.Collections.Generic;
using FiresecAPI;

namespace SKDDriver
{
	public abstract class WithShortTranslator<TableT, ApiT, FilterT, ShortT> : OrganisationElementTranslator<TableT, ApiT, FilterT>
		where TableT : class, DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement, new()
		where ApiT : OrganisationElementBase, new()
		where FilterT : OrganisationFilterBase
		where ShortT : new()
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
