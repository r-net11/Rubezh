using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using RubezhAPI;
using API = RubezhAPI.SKD;

namespace RubezhDAL.DataClasses
{
	public abstract class OrganisationShortTranslatorBase<TTableItem, TShort, TApiItem, TFilter> : ShortTranslatorBase<TTableItem, TShort, TApiItem, TFilter>
		where TTableItem : class, IOrganisationItem, new()
		where TApiItem : class, API.IOrganisationElement, new()
		where TShort : class, API.IOrganisationElement, new()
		where TFilter : API.OrganisationFilterBase
	{
		public OrganisationShortTranslatorBase(ITranslatorGet<TTableItem, TApiItem, TFilter> tranlsator) : base(tranlsator) { }

		public override IQueryable<TTableItem> GetTableItems()
		{
			return ParentTranslator.Table.Include(x => x.Organisation.Users);
		}

		public OperationResult<TShort> GetSingle(Guid? uid)
		{
			try
			{
				if (uid == null)
					return new OperationResult<TShort>(null);
				var tableItems = GetTableItems().Where(x => x.UID == uid.Value);
				var result = GetAPIItems(tableItems).FirstOrDefault();
				return new OperationResult<TShort>(result);
			}
			catch (System.Exception e)
			{
				return OperationResult<TShort>.FromError(e.Message);
			}
		}
	}
}