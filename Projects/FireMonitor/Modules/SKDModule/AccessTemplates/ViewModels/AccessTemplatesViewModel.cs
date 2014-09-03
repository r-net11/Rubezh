using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
    public class AccessTemplatesViewModel : CartothequeTabItemCopyPasteBase<AccessTemplate, AccessTemplateFilter, AccessTemplateViewModel, AccessTemplateDetailsViewModel>
	{
		protected override IEnumerable<AccessTemplate> GetModels(AccessTemplateFilter filter)
		{
			return AccessTemplateHelper.Get(filter);
		}
		protected override IEnumerable<AccessTemplate> GetModelsByOrganisation(Guid organisationUID)
		{
			return AccessTemplateHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(Guid uid)
		{
			return AccessTemplateHelper.MarkDeleted(uid);
		}
		protected override bool Save(AccessTemplate item)
		{
			return AccessTemplateHelper.Save(item);
		}
		
		protected override AccessTemplate CopyModel(AccessTemplate source, bool newName = true)
		{
			var copy = base.CopyModel(source, newName);
			copy.Description = source.Description;
			foreach (var cardDoor in source.CardDoors)
			{
				var copyCardDoor = new CardDoor();
				copyCardDoor.DoorUID = cardDoor.DoorUID;
				copyCardDoor.EnterIntervalType = cardDoor.EnterIntervalType;
				copyCardDoor.EnterIntervalID = cardDoor.EnterIntervalID;
				copyCardDoor.ExitIntervalType = cardDoor.ExitIntervalType;
				copyCardDoor.ExitIntervalID = cardDoor.ExitIntervalID;
				copyCardDoor.CardUID = null;
				copyCardDoor.AccessTemplateUID = null;
				copy.CardDoors.Add(copyCardDoor);
			}
			copy.CardDoors.ForEach(x => x.AccessTemplateUID = copy.UID);
			return copy;
		}

        protected override bool CanPaste()
        {
            return base.CanPaste() && ParentOrganisation.Organisation.UID == _clipboard.OrganisationUID;
        }
	}

	
}