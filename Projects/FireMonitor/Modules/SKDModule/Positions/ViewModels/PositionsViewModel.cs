using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.SKD;
using FiresecClient.SKDHelpers;
using Infrastructure;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class PositionsViewModel : OrganisationBaseViewModel<ShortPosition, PositionFilter, PositionViewModel, PositionDetailsViewModel>
	{
		public PositionsViewModel():base()
		{
			ServiceFactory.Events.GetEvent<NewPositionEvent>().Unsubscribe(OnNewPosition);
			ServiceFactory.Events.GetEvent<NewPositionEvent>().Subscribe(OnNewPosition);
		}
		
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

		public void OnNewPosition(ShortPosition model) 
		{
			var organisation = Organisations.FirstOrDefault(x => x.Organisation.UID == model.OrganisationUID);
			if (organisation != null)
			{
				var viewModel = new PositionViewModel();
				viewModel.InitializeModel(organisation.Organisation, model, this);
				organisation.AddChild(viewModel);
			}
		}
	}
}