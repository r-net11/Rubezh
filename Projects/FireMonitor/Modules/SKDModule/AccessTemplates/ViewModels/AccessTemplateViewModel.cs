using System.Collections.Generic;
using FiresecAPI.SKD;
using SKDModule.Common.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModel : CartothequeTabItemElementBase<AccessTemplateViewModel, AccessTemplate>
	{
		public CardDoorsViewModel CardDoorsViewModel { get; private set; }

		public override void InitializeOrganisation(Organisation organisation)
		{
			base.InitializeOrganisation(organisation);
			CardDoorsViewModel = new CardDoorsViewModel(new List<CardDoor>());
		}

		public override void InitializeModel(Organisation organisation, AccessTemplate accessTemplate)
		{
			base.InitializeModel(organisation, accessTemplate);
			Description = accessTemplate.Description;
			CardDoorsViewModel = new CardDoorsViewModel(accessTemplate.CardDoors);
		}

		public override void Update(AccessTemplate accessTemplate)
		{
			Description = accessTemplate.Description;
			CardDoorsViewModel.Update(accessTemplate.CardDoors);
			base.Update(accessTemplate);
		}
	}

}