using System.Collections.ObjectModel;
using FiresecAPI;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient;
using System.Linq;
using System;
using System.Windows.Documents;
using System.Collections.Generic;
using Infrastructure.Common;
using FiresecClient.SKDHelpers;
using Infrastructure.Common.Windows;

namespace SKDModule.ViewModels
{
	public class DepartmentsViewModel : ViewPartViewModel
	{
		DepartmentFilter Filter;

		public DepartmentsViewModel()
		{
			EditFilterCommand = new RelayCommand(OnEditFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			Filter = new DepartmentFilter();
			Initialize();
		}

		void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter());
			var departments = DepartmentHelper.Get(Filter);

			OrganisationDepartments = new ObservableCollection<OrganisationDepartmentsViewModel>();
			foreach (var organisation in organisations)
			{
				var departmentViewModel = new OrganisationDepartmentsViewModel();
				departmentViewModel.Initialize(organisation, new List<Department>(departments.Where(x => x.OrganizationUID.Value == organisation.UID)));
				OrganisationDepartments.Add(departmentViewModel);
			}
			SelectedOrganisationDepartment = OrganisationDepartments.FirstOrDefault();
		}

		ObservableCollection<OrganisationDepartmentsViewModel> _organisationDepartments;
		public ObservableCollection<OrganisationDepartmentsViewModel> OrganisationDepartments
		{
			get { return _organisationDepartments; }
			set
			{
				_organisationDepartments = value;
				OnPropertyChanged("OrganisationDepartments");
			}
		}

		OrganisationDepartmentsViewModel _selectedOrganisationDepartment;
		public OrganisationDepartmentsViewModel SelectedOrganisationDepartment
		{
			get { return _selectedOrganisationDepartment; }
			set
			{
				_selectedOrganisationDepartment = value;
				OnPropertyChanged("SelectedOrganisationDepartment");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var departmentFilterViewModel = new DepartmentFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(departmentFilterViewModel))
			{
				Filter = departmentFilterViewModel.Filter;
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