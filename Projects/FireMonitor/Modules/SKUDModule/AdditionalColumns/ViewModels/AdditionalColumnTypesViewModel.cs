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
	public class AdditionalColumnTypesViewModel : ViewPartViewModel
	{
		AdditionalColumnTypeFilter Filter;

		public AdditionalColumnTypesViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new AdditionalColumnTypeFilter();
			Initialize();
		}

		void Initialize()
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { Uids = FiresecManager.CurrentUser.OrganisationUIDs });
			var additionalColumnTypes = AdditionalColumnTypeHelper.Get(Filter);
			if(additionalColumnTypes == null)
				additionalColumnTypes = new List<AdditionalColumnType>();
			OrganisationAdditionalColumnTypes = new ObservableCollection<OrganisationAdditionalColumnTypesViewModel>();
			foreach (var organisation in organisations)
			{
				var additionalColumnTypeViewModel = new OrganisationAdditionalColumnTypesViewModel();
				additionalColumnTypeViewModel.Initialize(organisation, new List<AdditionalColumnType>(additionalColumnTypes.Where(x=>x.OrganizationUID.Value == organisation.UID)));
				OrganisationAdditionalColumnTypes.Add(additionalColumnTypeViewModel);
			}
			SelectedOrganisationAdditionalColumnType = OrganisationAdditionalColumnTypes.FirstOrDefault();
		}

		ObservableCollection<OrganisationAdditionalColumnTypesViewModel> _organisationAdditionalColumnTypes;
		public ObservableCollection<OrganisationAdditionalColumnTypesViewModel> OrganisationAdditionalColumnTypes
		{
			get { return _organisationAdditionalColumnTypes; }
			set
			{
				_organisationAdditionalColumnTypes = value;
				OnPropertyChanged(()=>OrganisationAdditionalColumnTypes);
			}
		}

		OrganisationAdditionalColumnTypesViewModel _selectedOrganisationAdditionalColumn;
		public OrganisationAdditionalColumnTypesViewModel SelectedOrganisationAdditionalColumnType
		{
			get { return _selectedOrganisationAdditionalColumn; }
			set
			{
				_selectedOrganisationAdditionalColumn = value;
				OnPropertyChanged(() => SelectedOrganisationAdditionalColumnType);
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var additionalColumnTypeFilterViewModel = new AdditionalColumnTypeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(additionalColumnTypeFilterViewModel))
			{
				Filter = additionalColumnTypeFilterViewModel.Filter;
				Initialize();
			}
		}
	}
}