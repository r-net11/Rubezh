using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using System.Linq;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using FiresecClient;
using System;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class AdditionalColumnsViewModel : ViewPartViewModel
	{
		public AdditionalColumnsViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			Initialize();
		}

		void Initialize()
		{
			var organisations = FiresecManager.GetOrganizations(new OrganizationFilter());
			//var additionalColumns = FiresecManager.GetAdditionalColumns(null);
			var additionalColumns = new List<AdditionalColumn>();

			OrganisationAdditionalColumns = new ObservableCollection<OrganisationAdditionalColumnsViewModel>();
			foreach (var organisation in organisations)
			{
				var additionalColumnViewModel = new OrganisationAdditionalColumnsViewModel();
				additionalColumnViewModel.Initialize(organisation.Name, new List<AdditionalColumn>(additionalColumns.Where(x=>x.OrganizationUid.Value == organisation.UID)));
				OrganisationAdditionalColumns.Add(additionalColumnViewModel);
			}
			SelectedOrganisationAdditionalColumn = OrganisationAdditionalColumns.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<OrganisationAdditionalColumnsViewModel> _organisationAdditionalColumns;
		public ObservableCollection<OrganisationAdditionalColumnsViewModel> OrganisationAdditionalColumns
		{
			get { return _organisationAdditionalColumns; }
			set
			{
				_organisationAdditionalColumns = value;
				OnPropertyChanged("OrganisationAdditionalColumns");
			}
		}

		OrganisationAdditionalColumnsViewModel _selectedOrganisationAdditionalColumn;
		public OrganisationAdditionalColumnsViewModel SelectedOrganisationAdditionalColumn
		{
			get { return _selectedOrganisationAdditionalColumn; }
			set
			{
				_selectedOrganisationAdditionalColumn = value;
				OnPropertyChanged("SelectedOrganisationAdditionalColumn");
			}
		}
	}
}