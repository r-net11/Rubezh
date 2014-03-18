using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using FiresecClient.SKDHelpers;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : ViewPartViewModel
	{
		public static EmployeesViewModel Current { get; private set; }

		public EmployeesViewModel()
		{
			RefreshCommand = new RelayCommand(OnRefresh);
			EditFilterCommand = new RelayCommand(OnEditFilter);
			Filter = new EmployeeFilter();
			Initialize();
		}

		EmployeeFilter Filter;

		void Initialize()
		{
			var organisations = OrganizationHelper.Get(new OrganizationFilter());
			var employees = EmployeeHelper.Get(Filter);

			OrganisationEmployees = new ObservableCollection<OrganisationEmployeesViewModel>();
			foreach (var organisation in organisations)
			{
				var employeeViewModel = new OrganisationEmployeesViewModel();
				employeeViewModel.Initialize(organisation, new List<Employee>(employees.Where(x => x.OrganizationUID != null && x.OrganizationUID.Value == organisation.UID)));
				OrganisationEmployees.Add(employeeViewModel);
			}
			SelectedOrganisationEmployee = OrganisationEmployees.FirstOrDefault();
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		ObservableCollection<OrganisationEmployeesViewModel> _organisationEmployees;
		public ObservableCollection<OrganisationEmployeesViewModel> OrganisationEmployees
		{
			get { return _organisationEmployees; }
			set
			{
				_organisationEmployees = value;
				OnPropertyChanged("OrganisationEmployees");
			}
		}

		OrganisationEmployeesViewModel _selectedOrganisationEmployee;
		public OrganisationEmployeesViewModel SelectedOrganisationEmployee
		{
			get { return _selectedOrganisationEmployee; }
			set
			{
				_selectedOrganisationEmployee = value;
				OnPropertyChanged("SelectedOrganisationEmployee");
			}
		}

		public RelayCommand EditFilterCommand { get; private set; }
		void OnEditFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
				Initialize();
			}
		}
	}
}