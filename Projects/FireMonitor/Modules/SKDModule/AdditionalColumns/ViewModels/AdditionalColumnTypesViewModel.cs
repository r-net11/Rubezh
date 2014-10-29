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
		protected override bool MarkDeleted(ShortAdditionalColumnType model)
		{
			return AdditionalColumnTypeHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortAdditionalColumnType model)
		{
			return AdditionalColumnTypeHelper.Restore(model);
		}
		protected override bool Add(ShortAdditionalColumnType item)
		{
			var additionalColumnType = AdditionalColumnTypeHelper.GetDetails(_clipboardUID);
			additionalColumnType.UID = item.UID;
			additionalColumnType.Description = item.Description;
			additionalColumnType.Name = item.Name;
			additionalColumnType.OrganisationUID = item.OrganisationUID;
			return AdditionalColumnTypeHelper.Save(additionalColumnType, true);
		}

		protected override string ItemRemovingName
		{
			get { return "дополнительную колонку"; }
		}
	}
}