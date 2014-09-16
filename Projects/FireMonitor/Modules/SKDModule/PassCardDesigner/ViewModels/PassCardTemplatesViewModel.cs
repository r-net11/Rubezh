using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using SKDModule.ViewModels;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplatesViewModel : OrganisationBaseViewModel<ShortPassCardTemplate, PassCardTemplateFilter, PassCardTemplateViewModel, PassCardTemplateDetailsViewModel>
	{
		protected override IEnumerable<ShortPassCardTemplate> GetModels(PassCardTemplateFilter filter)
		{
			return PassCardTemplateHelper.Get(filter);
		}
		protected override IEnumerable<ShortPassCardTemplate> GetModelsByOrganisation(Guid organisationUID)
		{
			return PassCardTemplateHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(Guid uid)
		{
			return PassCardTemplateHelper.MarkDeleted(uid);
		}
		protected override bool Save(ShortPassCardTemplate item)
		{
			var position = PassCardTemplateHelper.GetDetails(item.UID);
			position.UID = item.UID;
			position.Description = item.Description;
			position.Caption = item.Name;
			position.OrganisationUID = item.OrganisationUID;
			return PassCardTemplateHelper.Save(position);
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон пропуска"; }
		}
	}
}
