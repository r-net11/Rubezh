using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using Infrastructure.Common.Windows;
using GKProcessor;

namespace SKUDModule.ViewModels
{
	public class SKUDViewModel : ViewPartViewModel
	{
		public SKUDViewModel()
		{
			//Employees = new ObservableCollection<EmployeeViewModel>();
			//SelectedEmployee = Employees.FirstOrDefault();
			//AddCommand = new RelayCommand(OnAdd);
			//RemoveCommand = new RelayCommand(OnRemove, CanEditDelete);
			//EditCommand = new RelayCommand(OnEdit, CanEditDelete);
		}

		//ObservableCollection<EmployeeViewModel> employees;
		//public ObservableCollection<EmployeeViewModel> Employees
		//{
		//    get{ return employees; }
		//    set 
		//    { 
		//        employees = value;
		//        OnPropertyChanged("Employees");
		//    }
		//}

		//EmployeeViewModel selectedEmployee;
		//public EmployeeViewModel SelectedEmployee
		//{
		//    get { return selectedEmployee; }
		//    set
		//    {
		//        selectedEmployee = value;
		//        OnPropertyChanged("SelectedEmployee");
		//    }
		//}


		//public RelayCommand AddCommand { get; private set; }
		//void OnAdd()
		//{
		//    var employeeDelailsViewModel = new EmployeeDetailsViewModel();
		//    if (DialogService.ShowModalWindow(employeeDelailsViewModel))
		//    {
		//        var employee = employeeDelailsViewModel.Employee;
		//        Employees.Add(new EmployeeViewModel(employee));
		//        //TestDataHelper.AddEmployee(employee);
		//    }
		//}

		//public RelayCommand RemoveCommand { get; private set; }
		//void OnRemove()
		//{
		//    //TestDataHelper.RemoveEmployee(SelectedEmployee.Employee);
		//    Employees.Remove(SelectedEmployee);
		//    SelectedEmployee = Employees.FirstOrDefault();
			
		//}

		//public RelayCommand EditCommand { get; private set; }
		//void OnEdit()
		//{
		//    var employeeDelailsViewModel = new EmployeeDetailsViewModel(SelectedEmployee.Employee);
		//    DialogService.ShowModalWindow(employeeDelailsViewModel);
		//    var employee = employeeDelailsViewModel.Employee;
		//    //Employees.Add(new EmployeeViewModel(employee));
		//    //TestDataHelper.AddEmployee(employee);
		//}

		//bool CanEditDelete()
		//{
		//    return SelectedEmployee != null;
		//}
	}
}
