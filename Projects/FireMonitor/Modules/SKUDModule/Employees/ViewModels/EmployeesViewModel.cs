using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using SKDModule.Events;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : ViewPartViewModel
	{
		public static EmployeesViewModel Current { get; private set; }
		EmployeeFilter Filter;
		public PersonType PersonType { get; private set; }
		public Organisation Organization { get; private set; }
		public List<AdditionalColumnType> AdditionalColumnTypes { get; private set; }

		public EmployeesViewModel()
		{
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			RefreshCommand = new RelayCommand(OnRefresh);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);

			Filter = new EmployeeFilter();
			if (FiresecManager.CurrentUser.IsGuestsAllowed)
				Filter.PersonType = PersonType.Guest;
			else
				Filter.PersonType = PersonType.Employee;
			var organizationUID = FiresecManager.CurrentUser.OrganisationUIDs.FirstOrDefault();
			if (organizationUID != Guid.Empty)
				Filter.OrganizationUIDs = new List<Guid> { organizationUID };
			Initialize();
		}

		void Initialize()
		{
			PersonType = Filter.PersonType;

			Organization = OrganisationHelper.GetSingle(Filter.OrganizationUIDs.FirstOrDefault());
			var employees = EmployeeHelper.Get(Filter);
			Employees = new ObservableCollection<EmployeeViewModel>();
			foreach (var employee in employees)
			{
				var employeeViewModel = new EmployeeViewModel(Organization, employee);
				Employees.Add(employeeViewModel);
			}
			SelectedEmployee = Employees.FirstOrDefault();

			InitializeAdditionalColumns();
			ServiceFactory.Events.GetEvent<UpdateAdditionalColumns>().Publish(null);
		}

		ObservableCollection<EmployeeViewModel> _employee;
		public ObservableCollection<EmployeeViewModel> Employees
		{
			get { return _employee; }
			set
			{
				_employee = value;
				OnPropertyChanged("Employees");
			}
		}

		EmployeeViewModel _selectedEmployee;
		public EmployeeViewModel SelectedEmployee
		{
			get { return _selectedEmployee; }
			set
			{
				_selectedEmployee = value;
				OnPropertyChanged("SelectedEmployee");
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(this);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employeeViewModel = new EmployeeViewModel(Organization, employeeDetailsViewModel.ShortEmployee);
				Employees.Add(employeeViewModel);
				SelectedEmployee = employeeViewModel;
			}
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(this, SelectedEmployee.ShortEmployee);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.ShortEmployee);
			}
		}
		bool CanEdit()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var employee = SelectedEmployee.ShortEmployee;
			var removeResult = EmployeeHelper.MarkDeleted(employee.UID);
			if (!removeResult)
				return;

			var index = Employees.IndexOf(SelectedEmployee);
			Employees.Remove(SelectedEmployee);
			index = Math.Min(index, Employees.Count - 1);
			if (index > -1)
				SelectedEmployee = Employees[index];
		}
		bool CanRemove()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
				Initialize();
			}
		}

		public RelayCommand RefreshCommand { get; private set; }
		void OnRefresh()
		{
			Initialize();
		}

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var additionalColumnTypeFilter = new AdditionalColumnTypeFilter();
			if (Organization != null)
				additionalColumnTypeFilter.OrganizationUIDs.Add(Organization.UID);
			var columnTypes = AdditionalColumnTypeHelper.Get(additionalColumnTypeFilter);
			if (columnTypes == null)
				return;
			AdditionalColumnTypes = columnTypes.ToList();
			foreach (var additionalColumnType in AdditionalColumnTypes)
			{
				if (additionalColumnType.DataType == AdditionalColumnDataType.Text && additionalColumnType.IsInGrid)
					AdditionalColumnNames.Add(additionalColumnType.Name);
			}
			foreach (var employee in Employees)
			{
				employee.AdditionalColumnValues = new ObservableCollection<string>();
				foreach (var additionalColumnType in AdditionalColumnTypes)
				{
					if (additionalColumnType.DataType == AdditionalColumnDataType.Text)
						employee.AdditionalColumnValues.Add("Test " + additionalColumnType.Name);
				}
			}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }
	}
}