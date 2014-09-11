using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDModule.ViewModels;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;

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
			var passCardTemplate = new PassCardTemplate
			{
				UID = item.UID,
				Description = item.Description,
				Caption = item.Name,
				OrganisationUID = item.OrganisationUID
			};
			return PassCardTemplateHelper.Save(passCardTemplate);
		}

		protected override string ItemRemovingName
		{
			get { return "шаблон пропуска"; }
		}
	}
}
