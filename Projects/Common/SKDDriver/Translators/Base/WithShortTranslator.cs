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
		protected WithShortTranslator(SKDDatabaseService databaseService)
			: base(databaseService)
		{
		}

		public virtual OperationResult<IEnumerable<TShort>> GetList(TFilter filter)
		{
			try
			{
				BeforeGetList();
				var result = GetTableItems(filter).Select(TranslateToShort).ToList();
				var operationResult = new OperationResult<IEnumerable<TShort>>(result);
				AfterGetList();
				return operationResult;
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<TShort>>.FromError(e.Message);
			}
		}

		public virtual OperationResult<IEnumerable<TApiType>> GetFullList(TFilter filter)
		{
			try
			{
				BeforeGetList();
				var result = GetTableItems(filter).Select(Translate).ToList();
				var operationResult = new OperationResult<IEnumerable<TApiType>>(result);
				AfterGetList();
				return operationResult;
			}
			catch (Exception e)
			{
				return OperationResult<IEnumerable<TApiType>>.FromError(e.Message);
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
			var tableItem = Table.FirstOrDefault(x => x.UID.Equals(uid.Value));

			return tableItem == null ? null : TranslateToShort(tableItem);
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