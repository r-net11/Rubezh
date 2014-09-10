using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
    public class AdditionalColumnTypesViewModel : OrganisationBaseViewModel<ShortAdditionalColumnType, AdditionalColumnTypeFilter, AdditionalColumnTypeViewModel, AdditionalColumnTypeDetailsViewModel>
	{
        protected override IEnumerable<ShortAdditionalColumnType> GetModels(AdditionalColumnTypeFilter filter)
        {
            return AdditionalColumnTypeHelper.Get(filter);
        }
        protected override IEnumerable<ShortAdditionalColumnType> GetModelsByOrganisation(Guid organisationUID)
        {
            return AdditionalColumnTypeHelper.GetShortByOrganisation(organisationUID);
        }
        protected override bool MarkDeleted(Guid uid)
        {
            return AdditionalColumnTypeHelper.MarkDeleted(uid);
        }
        protected override bool Save(ShortAdditionalColumnType item)
        {
            var AdditionalColumnType = new AdditionalColumnType
            {
                UID = item.UID,
                Description = item.Description,
                Name = item.Name,
                OrganisationUID = item.OrganisationUID
            };
            return AdditionalColumnTypeHelper.Save(AdditionalColumnType);
        }

        protected override string ItemRemovingName
        {
            get { return "дополнительную колонку"; }
        }
	}
}