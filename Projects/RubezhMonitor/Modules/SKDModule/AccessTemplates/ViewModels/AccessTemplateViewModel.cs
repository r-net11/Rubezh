using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI.SKD;
using RubezhClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class AccessTemplateViewModel : OrganisationElementViewModel<AccessTemplateViewModel, AccessTemplate>, IDoorsParent
	{
		public CardDoorsViewModel CardDoorsViewModel { get; set; }
		
		public void InitializeDoors()
		{
			if(!IsOrganisation)
				CardDoorsViewModel = new CardDoorsViewModel(Model.CardDoors, this);
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
				var doorsUIDsToRemove = Model.CardDoors.Where(x => !doorUIDs.Any(y => y == x.DoorUID)).ToList();
				doorsUIDsToRemove.ForEach(x => Model.CardDoors.Remove(x));
				var saveResult = AccessTemplateHelper.Save(Model, false);
				if (saveResult)
				{
					CardDoorsViewModel.UpdateDoors(doorUIDs);
				}
			}
		}
	}
}