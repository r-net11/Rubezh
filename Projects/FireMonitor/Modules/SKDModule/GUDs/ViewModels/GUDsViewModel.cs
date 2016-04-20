using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class GUDsViewModel : ViewPartViewModel
	{
		public GUDsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new GUDFilter();
			Initialize();
		}

		GUDFilter Filter;

		public void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter());
			var guds = GUDHelper.Get(Filter);

			OrganisationGUDs = new ObservableCollection<OrganisationGUDsViewModel>();
			foreach (var organisation in organisations)
			{
				var gudViewModel = new OrganisationGUDsViewModel();
				gudViewModel.Initialize(organisation, new List<GUD>(guds.Where(x => x.OrganizationUID != null && x.OrganizationUID.Value == organisation.UID)));
				OrganisationGUDs.Add(gudViewModel);
			}
			SelectedOrganisationGUD = OrganisationGUDs.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<OrganisationGUDsViewModel> _organisationGUDs;
		public ObservableCollection<OrganisationGUDsViewModel> OrganisationGUDs
		{
			get { return _organisationGUDs; }
			set
			{
				_organisationGUDs = value;
				OnPropertyChanged("OrganisationGUDs");
			}
		}

		OrganisationGUDsViewModel _selectedOrganisationGUD;
		public OrganisationGUDsViewModel SelectedOrganisationGUD
		{
			get { return _selectedOrganisationGUD; }
			set
			{
				_selectedOrganisationGUD = value;
				OnPropertyChanged("SelectedOrganisationGUD");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new GUDFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(filterViewModel))
			{
				Filter = filterViewModel.Filter;
				Initialize();
			}
		}
	}
}