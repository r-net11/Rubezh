using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;
using FiresecClient.SKDHelpers;

namespace SKDModule.ViewModels
{
	public class AccessTemplatesViewModel : ViewPartViewModel
	{
		AccessTemplateFilter Filter;

		public AccessTemplatesViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			Filter = new AccessTemplateFilter();
			Initialize();
		}

		public void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var accessTemplates = AccessTemplateHelper.Get(Filter);

			OrganisationAccessTemplates = new ObservableCollection<OrganisationAccessTemplatesViewModel>();
			foreach (var organisation in organisations)
			{
				var accessTemplateViewModel = new OrganisationAccessTemplatesViewModel();
				accessTemplateViewModel.Initialize(organisation, new List<AccessTemplate>(accessTemplates.Where(x => x.OrganisationUID != null && x.OrganisationUID.Value == organisation.UID)));
				OrganisationAccessTemplates.Add(accessTemplateViewModel);
			}
			SelectedOrganisationAccessTemplate = OrganisationAccessTemplates.FirstOrDefault();
		}

		ObservableCollection<OrganisationAccessTemplatesViewModel> _organisationAccessTemplates;
		public ObservableCollection<OrganisationAccessTemplatesViewModel> OrganisationAccessTemplates
		{
			get { return _organisationAccessTemplates; }
			set
			{
				_organisationAccessTemplates = value;
				OnPropertyChanged("OrganisationAccessTemplates");
			}
		}

		OrganisationAccessTemplatesViewModel _selectedOrganisationAccessTemplate;
		public OrganisationAccessTemplatesViewModel SelectedOrganisationAccessTemplate
		{
			get { return _selectedOrganisationAccessTemplate; }
			set
			{
				_selectedOrganisationAccessTemplate = value;
				OnPropertyChanged("SelectedOrganisationAccessTemplate");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var filterViewModel = new AccessTemplateFilterViewModel(Filter);
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