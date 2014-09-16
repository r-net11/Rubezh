using System;
using System.Collections.Generic;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class PositionsViewModel : OrganisationBaseViewModel<ShortPosition, PositionFilter, PositionViewModel, PositionDetailsViewModel>
	{
		protected override IEnumerable<ShortPosition> GetModels(PositionFilter filter)
		{
			return PositionHelper.Get(filter);
		}
		protected override IEnumerable<ShortPosition> GetModelsByOrganisation(Guid organisationUID)
		{
			return PositionHelper.GetByOrganisation(organisationUID);
		}
		protected override bool MarkDeleted(Guid uid)
		{
			return PositionHelper.MarkDeleted(uid);
		}
		protected override bool Save(ShortPosition item)
		{
			var position = PositionHelper.GetDetails(_clipboardUID);
			position.UID = item.UID;
			position.Description = item.Description;
			position.Name = item.Name;
			position.OrganisationUID = item.OrganisationUID;
			return PositionHelper.Save(position);
		}
        
        protected override string ItemRemovingName
        {
            get { return "должность"; }
        }
	}
}