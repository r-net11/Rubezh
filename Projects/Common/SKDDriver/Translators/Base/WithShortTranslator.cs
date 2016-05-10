using StrazhAPI;
using StrazhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StrazhDAL
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
				BeforeGetList();
				var result = new List<TShort>();
				foreach (var tableItem in GetTableItems(filter))
				{
					var employeeListItem = TranslateToShort(tableItem);
					result.Add(employeeListItem);
				}
				var operationResult = new OperationResult<IEnumerable<TShort>>(result);
				AfterGetList();
				return operationResult;
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<TShort>>.FromError(e.Message);
			}
		}

		protected virtual void BeforeGetList()
		{
		}

		protected virtual void AfterGetList()
		{
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
				OrganisationUID = tableItem.OrganisationUID != null ? tableItem.OrganisationUID.Value : Guid.Empty,
				RemovalDate = tableItem.RemovalDate
			};
		}
	}
}