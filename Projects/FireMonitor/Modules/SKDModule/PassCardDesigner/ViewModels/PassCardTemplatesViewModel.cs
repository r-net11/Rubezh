using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SKDModule.ViewModels;
using FiresecAPI.SKD;

namespace SKDModule.PassCardDesigner.ViewModels
{
	public class PassCardTemplatesViewModel : OrganisationBaseViewModel<ShortPassCardTemplate, PassCardTemplateFilter, PassCardTemplateViewModel, PassCardTemplateDetailsViewModel>
	{
		protected override IEnumerable<ShortPassCardTemplate> GetModels(PassCardTemplateFilter filter)
		{
			//return PassCardTemplateHelper.Get(filter);
			return Enumerable.Empty<ShortPassCardTemplate>();
		}
		protected override IEnumerable<ShortPassCardTemplate> GetModelsByOrganisation(Guid organisationUID)
		{
			//return PassCardTemplateHelper.GetByOrganisation(organisationUID);
			return Enumerable.Empty<ShortPassCardTemplate>();
		}
		protected override bool MarkDeleted(Guid uid)
		{
			//return PassCardTemplateHelper.MarkDeleted(uid);
			return true;
		}
		protected override bool Save(ShortPassCardTemplate item)
		{
			//var passCardTemplate = new PassCardTemplate
			//{
			//    UID = item.UID,
			//    Description = item.Description,
			//    Name = item.Name,
			//    OrganisationUID = item.OrganisationUID
			//};
			//return PassCardTemplateHelper.Save(passCardTemplate);
			return true;
		}

		protected override ShortPassCardTemplate CopyModel(ShortPassCardTemplate source)
		//{
			var copy = base.CopyModel(source);
		//    //copy.Description = source.Description;
		//    return copy;
		//}

		protected override string ItemRemovingName
		{
			get { return "шаблон пропуска"; }
		}
	}
}
