using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using FiresecClient.SKDHelpers;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class PositionsViewModel : ViewPartViewModel
	{
		PositionFilter Filter;

		public PositionsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			Filter = new PositionFilter();
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var positions = PositionHelper.Get(Filter);

			OrganisationPositions = new ObservableCollection<OrganisationPositionsViewModel>();
			foreach (var organisation in organisations)
			{
				var positionViewModel = new OrganisationPositionsViewModel();
				positionViewModel.Initialize(organisation, new List<Position>(positions.Where(x => x.OrganizationUID != null && x.OrganizationUID.Value == organisation.UID)));
				OrganisationPositions.Add(positionViewModel);
			}
			SelectedOrganisationPosition = OrganisationPositions.FirstOrDefault();
		}

		ObservableCollection<OrganisationPositionsViewModel> _organisationPositions;
		public ObservableCollection<OrganisationPositionsViewModel> OrganisationPositions
		{
			get { return _organisationPositions; }
			set
			{
				_organisationPositions = value;
				OnPropertyChanged("OrganisationPositions");
			}
		}

		OrganisationPositionsViewModel _selectedOrganisationPosition;
		public OrganisationPositionsViewModel SelectedOrganisationPosition
		{
			get { return _selectedOrganisationPosition; }
			set
			{
				_selectedOrganisationPosition = value;
				OnPropertyChanged("SelectedOrganisationPosition");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new PositionFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				Initialize();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}
	}
}