using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI;
using FiresecAPI.SKD;

namespace SKDDriver
{
	public abstract class WithShortTranslator<TTableType, TApiType, TFilter, TShort> : OrganisationElementTranslator<TTableType, TApiType, TFilter>
		where TTableType : class, DataAccess.IOrganisationDatabaseElement, DataAccess.IDatabaseElement, DataAccess.IIsDeletedDatabaseElement, new()
		where TApiType : OrganisationElementBase, new()
		where TFilter : OrganisationFilterBase
		where TShort : class, IOrganisationElement, new() 
	{
		public WithShortTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{

		}

		public virtual OperationResult<IEnumerable<TShort>> GetList(TFilter filter)
		{
			try
			{
				var result = new List<TShort>();
				foreach (var tableItem in GetTableItems(filter))
				{
					var employeeListItem = TranslateToShort(tableItem);
					result.Add(employeeListItem);
				}
				var operationResult = new OperationResult<IEnumerable<TShort>>();
				operationResult.Result = result;
				return operationResult;
			}
			catch (Exception e)
			{
				return new OperationResult<IEnumerable<TShort>>(e.Message);
			}
		}

		public TShort GetSingleShort(Guid? uid)
		{
			if (uid == null)
				return null;
			var tableItem = Table.Where(x => x.UID.Equals(uid.Value)).FirstOrDefault();
			if (tableItem == null)
				return null;
			return TranslateToShort(tableItem);
		}

		protected virtual TShort TranslateToShort(TTableType tableItem)
		{
			return new TShort
			{
				UID = tableItem.UID,
				IsDeleted = tableItem.IsDeleted,
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty
			};
		}
	}
}