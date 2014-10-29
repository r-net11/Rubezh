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
		protected override bool MarkDeleted(ShortPassCardTemplate model)
		{
			return PassCardTemplateHelper.MarkDeleted(model);
		}
		protected override bool Restore(ShortPassCardTemplate model)
		{
			return PassCardTemplateHelper.Restore(model);
		}
		protected override bool Add(ShortPassCardTemplate item)
		{
			var passCardTemplate = PassCardTemplateHelper.GetDetails(_clipboardUID);
			passCardTemplate.UID = item.UID;
			passCardTemplate.Description = item.Description;
			passCardTemplate.Caption = item.Name;
			passCardTemplate.OrganisationUID = item.OrganisationUID;
			return PassCardTemplateHelper.Save(passCardTemplate, true);
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон пропуска"; }
		}
	}
}
