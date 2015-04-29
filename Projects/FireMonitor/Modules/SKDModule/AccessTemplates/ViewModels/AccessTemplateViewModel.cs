using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModel : OrganisationElementViewModel<AccessTemplateViewModel, AccessTemplate>, ICardDoorsParent
	{
		public CardDoorsViewModel CardDoorsViewModel { get; private set; }
		
		public override void InitializeOrganisation(Organisation organisation, ViewPartViewModel parentViewModel)
		{
			base.InitializeOrganisation(organisation, parentViewModel);
			CardDoorsViewModel = new CardDoorsViewModel(new List<CardDoor>(), this);
		}

		public override void InitializeModel(Organisation organisation, AccessTemplate accessTemplate, ViewPartViewModel parentViewModel)
		{
			base.InitializeModel(organisation, accessTemplate, parentViewModel);
			CardDoorsViewModel = new CardDoorsViewModel(accessTemplate.CardDoors, this);
		}

		public override void Update(AccessTemplate accessTemplate)
		{
			CardDoorsViewModel.Update(accessTemplate.CardDoors);
			base.Update(accessTemplate);
		}

		public void UpdateCardDoors(IEnumerable<Guid> doorUIDs)
		{
			if (!IsOrganisation)
			{
				Model.CardDoors.RemoveAll(x => doorUIDs.Any(y => y == x.DoorUID));
				AccessTemplateHelper.Save(Model, false);
			}
		}
	}
}