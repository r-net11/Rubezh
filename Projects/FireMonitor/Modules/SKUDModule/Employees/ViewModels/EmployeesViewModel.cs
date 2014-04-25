using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using FiresecClient.SKDHelpers;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : ViewPartViewModel
	{
		public PersonType PersonType { get; private set; }
		public List<ShortAdditionalColumnType> AdditionalColumnTypes { get; private set; }

		public EmployeesViewModel()
		{
			AddCommand = new RelayCommand(OnAdd, CanAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		public void Initialize(EmployeeFilter filter)
		{
			var organisations = OrganisationHelper.Get(new OrganisationFilter() { UIDs = FiresecManager.CurrentUser.OrganisationUIDs });
			var employees = EmployeeHelper.Get(filter);
			PersonType = filter.PersonType;
			AllEmployees = new List<EmployeeViewModel>();
			Organisations = new List<EmployeeViewModel>();
			foreach (var organisation in organisations)
			{
				var organisationViewModel = new EmployeeViewModel(organisation);
				Organisations.Add(organisationViewModel);
				AllEmployees.Add(organisationViewModel);
				foreach (var employee  in employees)
				{
					if (employee.OrganisationUID == organisation.UID)
					{
						var employeeViewModel = new EmployeeViewModel(organisation, employee);
						organisationViewModel.AddChild(employeeViewModel);
						AllEmployees.Add(employeeViewModel);
					}
				}
			}
			OnPropertyChanged("Organisations");
			SelectedEmployee = Organisations.FirstOrDefault();

			InitializeAdditionalColumns();
		}

		public List<EmployeeViewModel> Organisations { get; private set; }
		List<EmployeeViewModel> AllEmployees { get; set; }

		public void Select(Guid employeeUID)
		{
			if (employeeUID != Guid.Empty)
			{
				var employeeViewModel = AllEmployees.FirstOrDefault(x => x.ShortEmployee != null && x.ShortEmployee.UID == employeeUID);
				if (employeeViewModel != null)
					employeeViewModel.ExpandToThis();
				SelectedEmployee = employeeViewModel;
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
				OnPropertyChanged("IsEmployeeSelected");
			}
		}

		public bool IsEmployeeSelected
		{
			get { return SelectedEmployee != null && !SelectedEmployee.IsOrganisation; }
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(PersonType, SelectedEmployee.Organisation);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employeeViewModel = new EmployeeViewModel(SelectedEmployee.Organisation, employeeDetailsViewModel.ShortEmployee);
				AllEmployees.Add(employeeViewModel);
				SelectedEmployee = employeeViewModel;
			}
		}
		bool CanAdd()
		{
			return SelectedEmployee != null;
		}

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(PersonType, SelectedEmployee.Organisation, SelectedEmployee.ShortEmployee);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.ShortEmployee);
			}
		}
		bool CanEdit()
		{
			return SelectedEmployee != null && SelectedEmployee.Parent != null && !SelectedEmployee.IsOrganisation;
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
			var employee = SelectedEmployee.ShortEmployee;
			var removeResult = EmployeeHelper.MarkDeleted(employee.UID);
			if (!removeResult)
				return;

			var index = AllEmployees.IndexOf(SelectedEmployee);
			AllEmployees.Remove(SelectedEmployee);
			index = Math.Min(index, AllEmployees.Count - 1);
			if (index > -1)
				SelectedEmployee = AllEmployees[index];
		}
		bool CanRemove()
		{
			return SelectedEmployee != null && !SelectedEmployee.IsOrganisation;
		}

		public void InitializeAdditionalColumns()
		{
			AdditionalColumnNames = new ObservableCollection<string>();
			var additionalColumnTypeFilter = new AdditionalColumnTypeFilter() { OrganisationUIDs = FiresecManager.CurrentUser.OrganisationUIDs };
			var columnTypes = AdditionalColumnTypeHelper.Get(additionalColumnTypeFilter);
			if (columnTypes == null)
				return;
			AdditionalColumnTypes = columnTypes.ToList();
			foreach (var additionalColumnType in AdditionalColumnTypes)
			{
				//if (additionalColumnType.DataType == AdditionalColumnDataType.Text && additionalColumnType.IsInGrid)
				if (additionalColumnType.DataType == AdditionalColumnDataType.Text)
					AdditionalColumnNames.Add(additionalColumnType.Name);
			}
			foreach (var employee in AllEmployees)
			{
				employee.AdditionalColumnValues = new ObservableCollection<string>();
				foreach (var additionalColumnType in AdditionalColumnTypes)
				{
					if (additionalColumnType.DataType == AdditionalColumnDataType.Text)
						employee.AdditionalColumnValues.Add(additionalColumnType.Name + "/" + employee.Name);
				}
			}
		}

		public ObservableCollection<string> AdditionalColumnNames { get; private set; }
	}
}