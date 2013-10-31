using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using SKUDModule.Models;

namespace SKUDModule.ViewModels
{
	public class SKUDViewModel : ViewPartViewModel
	{
		public void Initialize()
		{
			Employees = new ObservableCollection<EmployeeViewModel>();
			TestHelper.GenerateTestData().ForEach(x => Employees.Add(new EmployeeViewModel(x)));
		}

		ObservableCollection<EmployeeViewModel> employees;
		public ObservableCollection<EmployeeViewModel> Employees
		{
			get{ return employees; }
			set 
			{ 
				employees = value;
				OnPropertyChanged("Employees");
			}
		}
	}
}
