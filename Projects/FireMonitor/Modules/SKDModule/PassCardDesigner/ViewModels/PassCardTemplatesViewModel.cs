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
		protected override bool Restore(Guid uid)
		{
			return PassCardTemplateHelper.Restore(uid);
		}
		protected override bool Save(ShortPassCardTemplate item)
		{
			var passCardTemplate = PassCardTemplateHelper.GetDetails(_clipboardUID);
			passCardTemplate.UID = item.UID;
			passCardTemplate.Description = item.Description;
			passCardTemplate.Caption = item.Name;
			passCardTemplate.OrganisationUID = item.OrganisationUID;
			return PassCardTemplateHelper.Save(passCardTemplate);
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон пропуска"; }
		}
	}
}
