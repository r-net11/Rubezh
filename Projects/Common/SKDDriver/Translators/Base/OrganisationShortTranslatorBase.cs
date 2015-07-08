using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using FiresecAPI;
using API = FiresecAPI.SKD;

namespace SKDDriver.DataClasses
{
    public abstract class OrganisationShortTranslatorBase<TTableItem, TShort, TApiItem, TFilter> : ShortTranslatorBase<TTableItem, TShort, TApiItem, TFilter>
        where TTableItem : class, IOrganisationItem, new()
        where TApiItem : class, API.IOrganisationElement, new()
        where TShort : class, API.IOrganisationElement, new()
        where TFilter : API.OrganisationFilterBase
    {
        public OrganisationShortTranslatorBase(ITranslatorGet<TTableItem, TApiItem, TFilter> tranlsator) : base(tranlsator) { }

        public override TShort Translate(TTableItem tableItem)
        {
            if (tableItem == null)
                return null;
            return new TShort
            {
                UID = tableItem.UID,
                Name = tableItem.Name,
                Description = tableItem.Description,
                IsDeleted = tableItem.IsDeleted,
                RemovalDate = tableItem.RemovalDate.GetValueOrDefault(),
                OrganisationUID = tableItem.OrganisationUID.GetValueOrDefault()
            };
        }

        public override IQueryable<TTableItem> GetTableItems()
        {
            return ParentTranslator.Table.Include(x => x.Organisation.Users);
        }
    }
}
