using System;
using System.Collections.Generic;
using System.Linq;
using StrazhAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModel : OrganisationElementViewModel<AccessTemplateViewModel, AccessTemplate>, IDoorsParent
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

		public void UpdateCardDoors(IEnumerable<Guid> doorUIDs, Guid organisationUID)
		{
			if (IsOrganisation || OrganisationUID != organisationUID) return;
			var doorsUIDsToRemove = Model.CardDoors.Where(x => doorUIDs.All(y => y != x.DoorUID)).ToList();
			doorsUIDsToRemove.ForEach(x => Model.CardDoors.Remove(x));
			var saveResult = AccessTemplateHelper.Save(Model, false);
			if (saveResult)
			{
				CardDoorsViewModel.UpdateDoors(doorUIDs);
			}
		}
	}
}