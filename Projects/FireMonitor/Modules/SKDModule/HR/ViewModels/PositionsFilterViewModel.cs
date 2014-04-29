using System.Collections.Generic;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class PositionsFilterViewModel : BaseViewModel
	{
		public PositionsFilterViewModel(PositionFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var positions = PositionHelper.Get(new PositionFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });

			AllPositions = new List<PositionFilterItemViewModel>();
			Organisations = new List<PositionFilterItemViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new PositionFilterItemViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllPositions.Add(organisationViewModel);
				foreach (var position in positions)
				{
					if (position.OrganisationUID == organisation.UID)
					{
						var positionViewModel = new PositionFilterItemViewModel(position.Name, position.Description, position.UID);
						positionViewModel.IsChecked = filter.UIDs.Contains(position.UID);
						organisationViewModel.AddChild(positionViewModel);
						AllPositions.Add(positionViewModel);
					}
				}
			}
		}

		public List<PositionFilterItemViewModel> AllPositions;
		public List<PositionFilterItemViewModel> Organisations { get; private set; }

		public PositionFilterItemViewModel[] RootPositions
		{
			get { return Organisations.ToArray(); }
		}

		public PositionFilter Save()
		{
			var positionFilter = new PositionFilter();
			foreach (var position in AllPositions)
			{
				if (position.IsChecked && !position.IsOrganisation)
				{
					positionFilter.UIDs.Add(position.UID);
				}
			}
			return positionFilter;
		}
	}
}