using System.Collections.ObjectModel;
using System.Linq;
using FiresecAPI;
using FiresecClient;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using Infrastructure.Common.Windows.ViewModels;
using System;

namespace SKDModule.ViewModels
{
	public class EmployeesViewModel : ViewPartViewModel
	{
		public EmployeesViewModel()
		{
			Filter = new EmployeeFilter();
			SelectedEmployee = Employees.FirstOrDefault();
			ShowFilterCommand = new RelayCommand(OnShowFilter);
			AddCommand = new RelayCommand(OnAdd);
			RemoveCommand = new RelayCommand(OnRemove, CanRemove);
			EditCommand = new RelayCommand(OnEdit, CanEdit);
		}

		EmployeeFilter filter;
		public EmployeeFilter Filter
		{
			get { return filter; }
			set
			{
				filter = value;
				UpdateEmployees();
			}
		}

		ObservableCollection<EmployeeViewModel> employees;
		public ObservableCollection<EmployeeViewModel> Employees
		{
			get { return employees; }
			set
			{
				employees = value;
				OnPropertyChanged("Employees");
			}
		}

		EmployeeViewModel selectedEmployee;
		public EmployeeViewModel SelectedEmployee
		{
			get { return selectedEmployee; }
			set
			{
				selectedEmployee = value;
				OnPropertyChanged("SelectedEmployee");
			}
		}

		void UpdateEmployees()
		{
			Employees = new ObservableCollection<EmployeeViewModel>();
			FiresecManager.GetEmployees(Filter).ToList().ForEach(x => Employees.Add(new EmployeeViewModel(x)));
		}


		public RelayCommand ShowFilterCommand { get; private set; }
		void OnShowFilter()
		{
			var employeeFilterViewModel = new EmployeeFilterViewModel(Filter);
			if (DialogService.ShowModalWindow(employeeFilterViewModel))
			{
				Filter = employeeFilterViewModel.Filter;
			}
		}

		public RelayCommand AddCommand { get; private set; }
		void OnAdd()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel();
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				var employee = employeeDetailsViewModel.Employee;
				var employeeViewModel = new EmployeeViewModel(employee);
				Employees.Add(employeeViewModel);
				SelectedEmployee = employeeViewModel;
			}
		}

		public RelayCommand RemoveCommand { get; private set; }
		void OnRemove()
		{
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

		public RelayCommand EditCommand { get; private set; }
		void OnEdit()
		{
			var employeeDetailsViewModel = new EmployeeDetailsViewModel(SelectedEmployee.Employee);
			if (DialogService.ShowModalWindow(employeeDetailsViewModel))
			{
				SelectedEmployee.Update(employeeDetailsViewModel.Employee);
			}
		}
		bool CanEdit()
		{
			return SelectedEmployee != null;
		}
	}
}