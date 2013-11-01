using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Infrastructure.Common.Windows.ViewModels;
using SKUDModule.Models;

namespace SKUDModule.ViewModels
{
	public class EmployeeDetailsViewModel:SaveCancelDialogViewModel
	{
		public EmployeeDetailsViewModel(Employee employee = null)
		{
			if (employee != null)
				Employee = employee;
			else
				Employee = new Employee();
		}

		Employee employee;
		public Employee Employee
		{
			get { return employee; }
			set
			{
				employee = value;
				OnPropertyChanged("Employee");
			}
		}
	}
}
