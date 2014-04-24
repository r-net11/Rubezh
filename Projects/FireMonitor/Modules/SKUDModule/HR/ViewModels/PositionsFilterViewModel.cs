using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using FiresecAPI;
using FiresecClient.SKDHelpers;
using FiresecClient;

namespace SKDModule.ViewModels
{
	public class PositionsFilterViewModel : BaseViewModel
	{
		public PositionsFilterViewModel(PositionFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var positions = PositionHelper.Get(new PositionFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs });

			AllFilterEntities = new List<FilterEntityViewModel>();
			Organisations = new List<FilterEntityViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new FilterEntityViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllFilterEntities.Add(organisationViewModel);
				foreach (var position in positions)
				{
					if (position.OrganisationUID == organisation.UID)
					{
						var positionViewModel = new FilterEntityViewModel(position.Name, position.Description, position.UID);
						positionViewModel.IsChecked = filter.UIDs.Contains(position.UID);
						organisationViewModel.AddChild(positionViewModel);
						AllFilterEntities.Add(positionViewModel);
					}
				}
			}
		}

		public List<FilterEntityViewModel> AllFilterEntities;

		List<FilterEntityViewModel> _organisations;
		public List<FilterEntityViewModel> Organisations
		{
			get { return _organisations; }
			private set
			{
				_organisations = value;
				OnPropertyChanged("Organisations");
			}
		}

		public FilterEntityViewModel[] RootFilterEntities
		{
			get { return Organisations.ToArray(); }
		}

		public PositionFilter Save()
		{
			var positionFilter = new PositionFilter();
			foreach (var filterEntity in AllFilterEntities)
			{
				if (filterEntity.IsChecked && !filterEntity.IsOrganisation)
				{
					positionFilter.UIDs.Add(filterEntity.UID);
				}
			}
			return positionFilter;
		}
	}
}