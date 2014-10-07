using System.Collections.Generic;
using FiresecAPI.SKD;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModel : OrganisationElementViewModel<AccessTemplateViewModel, AccessTemplate>
	{
		public CardDoorsViewModel CardDoorsViewModel { get; private set; }
		
		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
			CardDoorsViewModel = new CardDoorsViewModel(new List<CardDoor>());
		}

		public override void InitializeModel(Organisation organisation, AccessTemplate accessTemplate, ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, accessTemplate, parentViewModel);
			CardDoorsViewModel = new CardDoorsViewModel(accessTemplate.CardDoors);
		}

		public override void Update(AccessTemplate accessTemplate)
		{
			CardDoorsViewModel.Update(accessTemplate.CardDoors);
			base.Update(accessTemplate);
		}
	}
}